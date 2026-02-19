using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource melodySource;
    [SerializeField] private AudioSource percussionSource;
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float chaseLayerFadeTime = 0.8f;

    private float melodyVolume = 1f;
    private float percussionVolume;
    private float targetPercussionVolume;
    private Coroutine currentFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (melodySource != null) melodySource.loop = true;
        if (percussionSource != null) percussionSource.loop = true;
    }

    private void Update()
    {
        float k = (GameSettings.I != null) ? GameSettings.I.Music.Value : 1f;
        if (melodySource != null) melodySource.volume = melodyVolume * k;
        if (percussionSource != null) percussionSource.volume = percussionVolume * k;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetChaseLayer(bool on)
    {
        targetPercussionVolume = on ? 1f : 0f;
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
        currentFade = StartCoroutine(LerpVolumes(0f, 1f, 0f, targetPercussionVolume, Mathf.Max(0.1f, fadeTime)));
    }

    // Плавно меняем громкости за time
    private IEnumerator LerpVolumes(float fromMelody, float toMelody, float fromPerc, float toPerc, float time, bool isMainFade = true)
    {
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
        yield return StartCoroutine(LerpVolumes(0f, 1f, 0f, targetPercussionVolume, duration, isMainFade: false));

        melodyVolume = 1f;
        percussionVolume = targetPercussionVolume;
        currentFade = null;
    }
}
