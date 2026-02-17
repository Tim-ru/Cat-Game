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
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddInteractable(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().RemoveInteractable(this);
        }
    }
}
