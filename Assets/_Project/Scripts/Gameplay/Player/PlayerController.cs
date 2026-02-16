using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 moveInput;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        Debug.Log("OnMove called: " + moveInput);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        
        if (moveInput.x != 0)
        {
            Debug.Log(moveInput.x);
        }
    }
}
