using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterCheckpointComponent : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private string _tag;
    [SerializeField] private Vector2 _spawnPosition;
    [SerializeField] private UnityEvent<Vector2> _triggerEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(_tag))
        {
            if (collision.gameObject.CompareTag(_tag))
            {
                _triggerEvent?.Invoke(_spawnPosition);
            }
        }
        else
        {
            if (collision.gameObject.layer == (1 << _layerMask))
            {
                _triggerEvent?.Invoke(_spawnPosition);
            }
        }
    }
}
