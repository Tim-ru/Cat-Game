using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new(0f, -0.5f);
    [SerializeField] private Sprite crouchSprite;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private Vector2 crouchColliderSize = new(0.75f, 0.3f);
    [SerializeField] private Vector2 crouchColliderOffset = new(0f, 0.05f);
    [SerializeField] private Vector2 standColliderSize = new(1f, 1f);
    [SerializeField] private Vector2 standColliderOffset = Vector2.zero;

    private const string GroundTag = "Ground";
    private Vector2 moveInput;
    private Vector2 velocity;
    private bool isCrouched;
    private float currentSpeed;
    private bool IsGrounded()
    {
        Vector2 point = new Vector2(transform.position.x, transform.position.y) + groundCheckOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(point, groundCheckRadius);

        foreach (Collider2D col in hits)
        {
            if (col.gameObject != gameObject && col.CompareTag(GroundTag))
                return true;
        }
        return false;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

        currentSpeed = maxSpeed;
        SetCrouching(false);
    }

    public void OnMove(Vector2 move)
    {
        moveInput = move;
    }

    public void OnJump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void SetCrouching(bool crouch)
    {
        isCrouched = crouch;

        if (spriteRenderer != null)
            spriteRenderer.sprite = crouch ? crouchSprite : idleSprite;

        currentSpeed = crouch ? crouchSpeed : maxSpeed;

        if (boxCollider != null)
        {
            boxCollider.size = crouch ? crouchColliderSize : standColliderSize;
            boxCollider.offset = crouch ? crouchColliderOffset : standColliderOffset;
        }
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, smoothTime);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 point = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(point, groundCheckRadius);
    }
}
