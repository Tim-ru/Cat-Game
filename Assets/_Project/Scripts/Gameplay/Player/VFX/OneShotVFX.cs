using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class OneShotVFX : MonoBehaviour
{
    [Header("Intensity scaling (0 = min, 1 = max) — только Count в Bursts")]
    [SerializeField] private int burstCountMin = 2;
    [SerializeField] private int burstCountMax = 50;
    [SerializeField] private float intensityPower = 2f;
    [SerializeField] private float destroyAfterSeconds = 2f;

    private ParticleSystem _particleSystem;
    private float spawnTime;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

      public void ApplyIntensity(float intensity)
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        intensity = Mathf.Clamp01(intensity);

        var emission = _particleSystem.emission;
        emission.enabled = true;
        if (emission.burstCount > 0)
        {
            float curve = Mathf.Pow(intensity, intensityPower);
            int count = Mathf.RoundToInt(Mathf.Lerp(burstCountMin, burstCountMax, curve));
            count = Mathf.Max(1, count);
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)count));
        }

        _particleSystem.Play();
        spawnTime = Time.time;
    }

  
    public void Play()
    {
        ApplyIntensity(0.5f);
    }

    private void Update()
    {
        if (Time.time - spawnTime < destroyAfterSeconds)
            return;
        if (!_particleSystem.isPlaying && _particleSystem.particleCount == 0)
        {
            Destroy(gameObject);
            return;
        }
        if (Time.time - spawnTime >= destroyAfterSeconds + 1f)
            Destroy(gameObject);
    }
}
