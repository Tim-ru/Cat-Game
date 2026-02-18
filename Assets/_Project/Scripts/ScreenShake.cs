using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    private CinemachineBasicMultiChannelPerlin _cam;
    private Coroutine _coroutine;
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

    public void ShakeCamera(float intensity, float timer)
    {
        _startingIntensity = intensity;
        _cam.AmplitudeGain = intensity;
        _timer = timer;
        _timerTotal = timer;
        _isShaking = true;
    }
    [ContextMenu("Shake")]
    public void Shake()
    {
        ShakeCamera(10, 5);
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
