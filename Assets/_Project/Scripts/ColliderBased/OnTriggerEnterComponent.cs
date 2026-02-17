using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterComponent : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private string _tag;
    [SerializeField] private UnityEvent<GameObject, GameObject> _triggerEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(_tag))
        {
            if (collision.gameObject.CompareTag(_tag))
            {
                _triggerEvent?.Invoke(collision.gameObject, gameObject);
            }
        }
        else
        {
            if (collision.gameObject.layer == (1 << _layerMask))
            {
                _triggerEvent?.Invoke(collision.gameObject, gameObject);
            }
        }
    }
}
