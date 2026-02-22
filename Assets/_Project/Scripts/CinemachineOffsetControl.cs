using Unity.Cinemachine;
using UnityEngine;

public class CinemachineOffsetControl : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;
    private CinemachineFollow _cinFollow;
    private bool _isChangingOffset;
    private Vector3 _currentOffset = Vector3.zero;
    private Vector3 _targetOffset = Vector3.zero;
    private float _currentDuration;
    void Start()
    {
        _cinFollow = GetComponent<CinemachineFollow>();
    }

    public void InverseXOffset()
    {
        _currentOffset = _cinFollow.FollowOffset;
        _targetOffset = _cinFollow.FollowOffset;
        _targetOffset.x = -_targetOffset.x;
        _currentDuration = _duration;
        _isChangingOffset = true;
    }

    private void Update()
    {
        if (!_isChangingOffset) return;
        _cinFollow.FollowOffset = Vector3.Lerp(_currentOffset, _targetOffset, 1 - (_currentDuration / _duration));
        _currentDuration -= Time.deltaTime;
        if (_currentDuration <= 0)
        {
            _isChangingOffset = false;
        }
    }
}
