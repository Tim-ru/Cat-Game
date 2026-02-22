using System;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public NoiseSettings[] _noises;
    public static ScreenShake Instance { get; private set; }
    private CinemachineBasicMultiChannelPerlin _cam;
    private IntPersistentProperty _screenShake;
    private float _timer;
    private float _timerTotal;
    private float _startingIntensity;
    private bool _isShaking = false;
    private void Awake()
    {
        Instance = this;
        CinemachineCamera camera = GetComponent<CinemachineCamera>();
        _cam = (CinemachineBasicMultiChannelPerlin)camera.GetCinemachineComponent(CinemachineCore.Stage.Noise);
    }
    private void Start()
    {
        _screenShake = GameSettings.I._screenShake;
        _screenShake.OnChanged += ShakeChanged;
    }

    private void ShakeChanged(int newValue, int oldValue)
    {
        if (newValue == 0)
        {
            _cam.AmplitudeGain = 0;
            _isShaking = false;
        }
    }

    public void ShakeCamera(float intensity, float timer)
    {
        if (GameSettings.I._screenShake.Value == 0) return;
        _startingIntensity = intensity;
        _cam.AmplitudeGain = intensity;
        _timer = timer;
        _timerTotal = timer;
        _isShaking = true;
    }
    [ContextMenu("Shake")]
    public void Shake()
    {
        if (GameSettings.I._screenShake.Value == 0) return;
        ShakeCamera(8, 1.5f);
    }
    [ContextMenu("Shake")]
    public void UpdateShakeForever(float intensity)
    {
        if (GameSettings.I._screenShake.Value == 0) return;
        _cam.AmplitudeGain = intensity;
    }
    public void ChangeShakePreset(int typeIndex)
    {
        if (GameSettings.I._screenShake.Value == 0) return;
        _cam.NoiseProfile = _noises[typeIndex];
    }
    public void RemoveShakeWithTime(float timer)
    {
        if (GameSettings.I._screenShake.Value == 0) return;
        _startingIntensity = _cam.AmplitudeGain;
        _timer = timer;
        _timerTotal = timer;
        _isShaking = true;
    }
    public void Update()
    {
        if (!_isShaking) return;

        if (_timer > 0)
        {
            _cam.AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, 1 - (_timer / _timerTotal));
            _timer -= Time.deltaTime;
            if (_timer <= 0)
                _cam.AmplitudeGain = 0;
        }
    }
}
