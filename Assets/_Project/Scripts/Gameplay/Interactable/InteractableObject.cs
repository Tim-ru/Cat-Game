using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent<GameObject> _event;
    public Vector2 Position => transform.position;

    public void Interact(GameObject interactor)
    {
        _event?.Invoke(interactor);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.AddInteractable(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.RemoveInteractable(this);
    }
}
