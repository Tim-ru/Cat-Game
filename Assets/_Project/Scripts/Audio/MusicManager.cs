using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource melodySource;
    [SerializeField] private AudioSource percussionSource;
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float chaseLayerFadeTime = 0.8f;
    private float percussionVolume;
    private float melodyVolume;
    private float targetPercussionVolume;
    private Coroutine currentFade;
    private bool _isVolumeChanging = false;
    private float maxMelodyVolume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        maxMelodyVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        melodyVolume = maxMelodyVolume;
        percussionVolume = 0f;
        targetPercussionVolume = percussionVolume;
    }

    private void Update()
    {
        if (_isVolumeChanging)
        {
            if (melodySource != null) melodySource.volume = melodyVolume;
            if (percussionSource != null) percussionSource.volume = percussionVolume;
        }
    }
    public void SetChaseLayer(bool on)
    {
        maxMelodyVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        targetPercussionVolume = on ? 1f : 0f;
        targetPercussionVolume = targetPercussionVolume > maxMelodyVolume ? maxMelodyVolume : targetPercussionVolume;
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(LerpVolumes(melodyVolume, melodyVolume, percussionVolume, targetPercussionVolume, chaseLayerFadeTime));
    }

    public void CrossfadeTo(AudioClip melody, AudioClip percussion, float duration = -1f)
    {
        if (duration < 0f) duration = fadeTime;
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(CrossfadeRoutine(melody, percussion, duration));
    }

    public void Play(AudioClip melody, AudioClip percussion = null)
    {
        if (melodySource != null && melody != null)
        {
            melodySource.clip = melody;
            melodySource.Play();
        }
        if (percussionSource != null && percussion != null)
        {
            percussionSource.clip = percussion;
            percussionSource.Play();
        }
        melodyVolume = 0f;
        percussionVolume = 0f;
        if (currentFade != null) StopCoroutine(currentFade);
        maxMelodyVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        currentFade = StartCoroutine(LerpVolumes(0f, maxMelodyVolume, 0f, targetPercussionVolume, Mathf.Max(0.1f, fadeTime)));
    }

    // Плавно меняем громкости за time
    private IEnumerator LerpVolumes(float fromMelody, float toMelody, float fromPerc, float toPerc, float time, bool isMainFade = true)
    {
        _isVolumeChanging = true;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = t / time;
            melodyVolume = Mathf.Lerp(fromMelody, toMelody, k);
            percussionVolume = Mathf.Lerp(fromPerc, toPerc, k);
            yield return null;
        }
        melodyVolume = toMelody;
        percussionVolume = toPerc;
        yield return null;
        _isVolumeChanging = false;
        if (isMainFade) currentFade = null;
    }

    private IEnumerator CrossfadeRoutine(AudioClip melody, AudioClip percussion, float duration)
    {
        // 1. Плавно убавить текущую музыку до нуля
        yield return StartCoroutine(LerpVolumes(melodyVolume, 0f, percussionVolume, 0f, duration, isMainFade: false));

        melodyVolume = 0f;
        percussionVolume = 0f;

        // 2. Поставить новый тречек и включить
        if (melodySource != null && melody != null)
        {
            melodySource.clip = melody;
            melodySource.Play();
        }
        if (percussionSource != null && percussion != null)
        {
            percussionSource.clip = percussion;
            percussionSource.Play();
        }

        // 3. Плавно прибавить волюма
        maxMelodyVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        yield return StartCoroutine(LerpVolumes(0f, maxMelodyVolume, 0f, targetPercussionVolume, duration, isMainFade: false));

        melodyVolume = maxMelodyVolume;
        percussionVolume = targetPercussionVolume;
        currentFade = null;
    }
}
