using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChangeLightComponent : MonoBehaviour
{
    private float _duration = 2.0f;
    private Light2D _light;
    private bool _isChanging = false;
    private float _targetOuter;
    private float _currentOuter;
    private float _startOuter;
    private float _currentTime;
    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _startOuter = _light.pointLightOuterRadius;
    }
    public void ChangeLight(float newOuter)
    {
        _isChanging = true;
        _currentTime = _duration;
        _targetOuter = newOuter;
        _startOuter = _light.pointLightOuterRadius;
        _currentOuter = _startOuter;
    }
    public void Duration(float newDir)
    {
        _duration = newDir;
    }

    private void Update()
    {
        if (!_isChanging) return;
        _currentOuter = Mathf.Lerp(_startOuter, _targetOuter, 1 - (_currentTime / _duration));
        _light.pointLightInnerRadius = _currentOuter - 2f;
        _light.pointLightOuterRadius = _currentOuter;
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0)
        {
            _isChanging = false;
            enabled = false;
        }
    }
}
