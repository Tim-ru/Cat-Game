using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public Vector2 Position => transform.position;

    public void Interact(GameObject interactor)
    {
        Debug.Log("InteractableObject interacted by " + interactor.name);
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
