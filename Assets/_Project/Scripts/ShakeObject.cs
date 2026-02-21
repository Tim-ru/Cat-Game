using System.Collections;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    [SerializeField] private float _shakeStrength;
    [SerializeField] private float _shakeIntensify;
    [SerializeField] private bool _isShaking;
    [SerializeField] private bool _isRotationShaking;
    private float _xPos;
    private float _yPos;
    private float _newXPos;
    private float _newYPos;
    private float _newZEuler;
    private float _waitingTime;
    private Coroutine _coroutine;

    private void Start()
    {
        _xPos = transform.position.x;
        _yPos = transform.position.y;
        _waitingTime = 1 / _shakeIntensify;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _xPos = transform.position.x;
        _yPos = transform.position.y;
        _waitingTime = 1 / _shakeIntensify;
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Shake());
    }
#endif

    private IEnumerator Shake()
    {
        while (_isShaking)
        {
            if (_isRotationShaking)
            {
                _newZEuler = transform.rotation.eulerAngles.z + _shakeStrength * Random.Range(-1f, 1f);
                if (_newZEuler <= 200f && _newZEuler >= 160f)
                    transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + _newZEuler);
            }
            else
            {
                _newXPos = _shakeStrength * Random.Range(-1f, 1f);
                _newYPos = _shakeStrength * Random.Range(-1f, 1f);
                transform.position = new Vector3(_xPos + _newXPos, _yPos + _newYPos, transform.position.z);
            }
            yield return new WaitForSeconds(_waitingTime);
        }
    }

    public void OnOff(bool status)
    {
        _isShaking = status;
    }
}
