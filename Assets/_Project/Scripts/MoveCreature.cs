using UnityEngine;

public class MoveCreature : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private Rigidbody2D rb;
    void FixedUpdate()
    {
        rb.linearVelocityX = -maxSpeed;
    }

    public void Teleport(Vector2 newPos)
    {
        transform.position = newPos;
        maxSpeed = 13f;
    }
}
