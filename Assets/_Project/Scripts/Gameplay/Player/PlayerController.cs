using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float smoothTime = 0.15f;
    [Tooltip("При скорости ниже этого порога и нулевом вводе — скорость обнуляется (избегаем дрифта и лишней физики).")]
    [SerializeField] private float velocityZeroThreshold = 0.01f;
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new(0f, -0.5f);
    [SerializeField] private Sprite crouchSprite;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    private float crouchSpeed = 1.5f;
    private float colliderTransitionSpeed = 6f;
    private Vector2 crouchColliderSize = new(0.82f, 0.35f);
    private Vector2 crouchColliderOffset = Vector2.zero;
    private Vector2 standColliderSize = new(0.82f, 0.6f);
    private Vector2 standColliderOffset = Vector2.zero;

    private float sprintSpeedMultiplier = 3f;
    private float longRunDurationToActivate = 3f;
    private float longRunMultiplier = 1.4f;
    private float longRunMinMoveInput = 0.1f;

    private Vector2 moveInput;
    private float longRunTimer;
    private bool wantsToSprint;
    private bool wasGrounded;
    private Vector2 velocity;
    private bool isCrouched;
    private float currentSpeed;
    private Vector2 colliderSizeVelocity;
    private Vector2 colliderOffsetVelocity;

    private const string GroundTag = "Ground";
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
        if (boxCollider != null)
        {
            boxCollider.size = standColliderSize;
            boxCollider.offset = standColliderOffset;
        }
        SetCrouching(false);
        wasGrounded = IsGrounded();
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
        if (crouch && !IsGrounded()) return;

        isCrouched = crouch;

        if (spriteRenderer != null)
            spriteRenderer.sprite = crouch ? crouchSprite : idleSprite;

        ApplyMovementSpeed();
    }

    public void SetSprinting(bool sprint)
    {
        wantsToSprint = sprint;
        ApplyMovementSpeed();
    }

    private void ApplyMovementSpeed()
    {
        bool grounded = IsGrounded();
        if (!grounded)
            return;

        bool isLongRunActive = longRunTimer >= longRunDurationToActivate && !isCrouched;

        if (isCrouched)
            currentSpeed = crouchSpeed;
        else if (wantsToSprint)
            currentSpeed = maxSpeed * sprintSpeedMultiplier;
        else if (isLongRunActive)
            currentSpeed = maxSpeed * longRunMultiplier;
        else
            currentSpeed = maxSpeed;
    }

    private void Update()
    {
        UpdateLongRunTimer();
    }

    private void UpdateLongRunTimer()
    {
        bool isMoving = Mathf.Abs(moveInput.x) >= longRunMinMoveInput;
        bool wasLongRunActive = longRunTimer >= longRunDurationToActivate;

        if (!isCrouched && isMoving)
            longRunTimer += Time.deltaTime;
        else
            longRunTimer = 0f;

        bool isLongRunActive = longRunTimer >= longRunDurationToActivate;
        if (wasLongRunActive != isLongRunActive)
            ApplyMovementSpeed();
    }

    public void OnInteract()
    {
        Debug.Log("Interact");
    }

    private void FixedUpdate()
    {
        bool grounded = IsGrounded();
        if (!wasGrounded && grounded)
            ApplyMovementSpeed();
        wasGrounded = grounded;

        if (boxCollider != null)
        {
            Vector2 targetSize = isCrouched ? crouchColliderSize : standColliderSize;
            Vector2 targetOffset = isCrouched ? crouchColliderOffset : standColliderOffset;
            float deltaTime = Time.fixedDeltaTime;

            boxCollider.size = Vector2.SmoothDamp(boxCollider.size, targetSize, ref colliderSizeVelocity, 1f / colliderTransitionSpeed, float.PositiveInfinity, deltaTime);
            boxCollider.offset = Vector2.SmoothDamp(boxCollider.offset, targetOffset, ref colliderOffsetVelocity, 1f / colliderTransitionSpeed, float.PositiveInfinity, deltaTime);
        }

        float targetVelX = moveInput.x * currentSpeed;
        Vector2 targetVelocity = new Vector2(targetVelX, rb.linearVelocity.y);

        // При остановке принудительно обнуляем скорость, чтобы не было микродвижений и лишних перерасчётов физики
        if (Mathf.Abs(targetVelX) < 0.001f && Mathf.Abs(rb.linearVelocity.x) < velocityZeroThreshold)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            velocity.x = 0f;
        }
        else
        {
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, smoothTime, float.PositiveInfinity, Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 point = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(point, groundCheckRadius);
    }
}
