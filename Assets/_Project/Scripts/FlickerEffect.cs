using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerEffect : MonoBehaviour
{
    [SerializeField] private Vector2 _waitingRange;
    [SerializeField] private float _minIntensify;
    [SerializeField] private bool _isStandartFleeking;
    [SerializeField] private bool _isOnOffFleeking;
    private float _onOffDuration;
    private Light2D _light;
    private Coroutine _coroutine;

    private void Start()
    {
        _light = GetComponent<Light2D>();
    }
    public void SetStandartFleaking()
    {
        _isStandartFleeking = true;
        _isOnOffFleeking = false;
        StopCoroutine(_coroutine);
        _light.intensity = 1;
    }

    public void SetOnOffModWithDuration(float duration)
    {
        _isStandartFleeking = false;
        _isOnOffFleeking = !_isOnOffFleeking;
        _onOffDuration = duration;
        StartOnOffCoroutine();
    }

    public void SetOnOffMod()
    {
        _isStandartFleeking = false;
        _isOnOffFleeking = !_isOnOffFleeking;
        StartOnOffCoroutine();
    }

    private void StartOnOffCoroutine()
    {
        if (_isOnOffFleeking)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(OnOffFleak());
        }
        else
        {
            StopCoroutine(_coroutine);
        }
    }
    private IEnumerator OnOffFleak()
    {
        while (_isOnOffFleeking)
        {
            _light.intensity = 0;
            yield return new WaitForSeconds(_onOffDuration);
            _light.intensity = 1;
            yield return new WaitForSeconds(_onOffDuration);
        }
    }

    private void Update()
    {
        if (!_isStandartFleeking) return;
        _light.intensity = Mathf.Lerp(_light.intensity, Random.Range(_waitingRange.x, _waitingRange.y), 0.1f);
    }
}
