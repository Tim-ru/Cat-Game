using System.Runtime.CompilerServices;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private bool _isRotating;
    [SerializeField] private float _rotationPerFrame;
    [SerializeField] private Vector2 _rotationRange;
    [SerializeField] private AnimationCurve _speedCurve;
    private float _currentRotation;
    private float _prevTargetRotation;
    private float _targetRotation;
    private int _rotationDirection = 1;

    private void Start()
    {
        _currentRotation = 0f;
        _prevTargetRotation = 0f;
        _targetRotation = Random.Range(_rotationRange.x, _rotationRange.y) * _rotationDirection;
    }
    private void FixedUpdate()
    {
        if (_isRotating)
        {
            _currentRotation += _rotationPerFrame * _speedCurve.Evaluate(_currentRotation / _targetRotation);
            transform.rotation = Quaternion.Euler(0, 0, 180f + _currentRotation * _rotationDirection + _prevTargetRotation);
            if (_currentRotation >= _targetRotation)
            {
                _prevTargetRotation = _prevTargetRotation < 0 ? -_prevTargetRotation : _prevTargetRotation;
                _prevTargetRotation = _currentRotation - _prevTargetRotation;
                _prevTargetRotation *= _rotationDirection;
                _targetRotation = _prevTargetRotation * _rotationDirection + Random.Range(_rotationRange.x, _rotationRange.y);
                _currentRotation = 0;
                _rotationDirection *= -1;
            }
        }
    }
}
