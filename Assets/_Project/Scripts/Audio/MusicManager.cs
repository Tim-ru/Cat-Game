using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource melodySource;
    [SerializeField] private AudioSource percussionSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private float fadeTime = 1.5f;
    [SerializeField] private float chaseLayerFadeTime = 0.8f;
    private float percussionVolume;
    private float melodyVolume;
    private float sfxVolume;
    private float targetPercussionVolume;
    private Coroutine currentFade;
    private bool _isVolumeChanging = false;
    private float maxVolume;
    private float maxSfxVolume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        maxVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        melodyVolume = maxVolume;
        percussionVolume = 0f;
        targetPercussionVolume = percussionVolume;
    }

    private void Update()
    {
        if (_isVolumeChanging)
        {
            if (melodySource != null) melodySource.volume = melodyVolume;
            if (percussionSource != null) percussionSource.volume = percussionVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }
    }
    public void SetChaseLayer(bool on)
    {
        maxVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        targetPercussionVolume = on ? 1f : 0f;
        targetPercussionVolume = targetPercussionVolume > maxVolume ? maxVolume : targetPercussionVolume;
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(LerpVolumes(melodyVolume, melodyVolume, percussionVolume, targetPercussionVolume, sfxVolume, sfxVolume, chaseLayerFadeTime));
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
        maxVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        currentFade = StartCoroutine(LerpVolumes(0f, maxVolume, 0f, targetPercussionVolume, sfxVolume, sfxVolume, Mathf.Max(0.1f, fadeTime)));
    }

    public void PlaySfx(AudioClip melody)
    {
        if (sfxSource != null && melody != null)
        {
            sfxSource.clip = melody;
            sfxSource.Play();
        }
        sfxVolume = 0f;
        if (currentFade != null) StopCoroutine(currentFade);
        maxSfxVolume = GameSettings.I.Sfx.Value * GameSettings.I.Master.Value;
        currentFade = StartCoroutine(LerpVolumes(melodyVolume, melodyVolume, percussionVolume, percussionVolume, 0f, maxSfxVolume, Mathf.Max(0.1f, fadeTime)));
    }

    // Плавно меняем громкости за time
    private IEnumerator LerpVolumes(float fromMelody, float toMelody, float fromPerc, float toPerc, float fromSfx, float toSfx, float time, bool isMainFade = true)
    {
        _isVolumeChanging = true;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float k = t / time;
            melodyVolume = Mathf.Lerp(fromMelody, toMelody, k);
            percussionVolume = Mathf.Lerp(fromPerc, toPerc, k);
            sfxVolume = Mathf.Lerp(fromSfx, toSfx, k);
            yield return null;
        }
        melodyVolume = toMelody;
        percussionVolume = toPerc;
        sfxVolume = toSfx;
        yield return null;
        _isVolumeChanging = false;
        if (isMainFade) currentFade = null;
    }

    private IEnumerator CrossfadeRoutine(AudioClip melody, AudioClip percussion, float duration)
    {
        // 1. Плавно убавить текущую музыку до нуля
        yield return StartCoroutine(LerpVolumes(melodyVolume, 0f, percussionVolume, 0f, sfxVolume, sfxVolume, duration, isMainFade: false));

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
        maxVolume = GameSettings.I.Music.Value * GameSettings.I.Master.Value;
        yield return StartCoroutine(LerpVolumes(0f, maxVolume, 0f, targetPercussionVolume, sfxVolume, sfxVolume, duration, isMainFade: false));

        melodyVolume = maxVolume;
        percussionVolume = targetPercussionVolume;
        currentFade = null;
    }

    public void StopCurrentSfx()
    {
        sfxSource.Stop();
    }
}
