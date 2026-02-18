using Unity.Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] float _orthographicSize = 4f;
    [SerializeField] float _defaultSize = 6.25f;
    [SerializeField] float _deltaSize = 0.005f;
    private bool _isChangingSize = false;
    private float _targetSize;
    private float _currentSize;
    private CinemachineCamera cam;
    private void Start()
    {
        cam = FindAnyObjectByType<CinemachineCamera>();
        _currentSize = _defaultSize;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cam != null)
        {
            _targetSize = _orthographicSize;
            _isChangingSize = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cam != null)
        {
            _targetSize = _defaultSize;
            _isChangingSize = true;
        }
    }

    private void Update()
    {
        if (!_isChangingSize) return;

        if (_currentSize <= _targetSize)
        {
            _currentSize += _deltaSize;
            if (_currentSize >= _targetSize)
                _isChangingSize = false;
        }
        else
        {
            _currentSize -= _deltaSize;
            if (_currentSize <= _targetSize)
                _isChangingSize = false;
        }
        cam.Lens.OrthographicSize = _currentSize;
    }
}
