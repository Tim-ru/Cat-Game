using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float groundLerpSpeed = 10f;
    [SerializeField] private float airLerpSpeed = 2f;
    [SerializeField] private float velocityZeroThreshold = 0.01f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float airControlMultiplier = 0.65f;
    [SerializeField] private float gravityScaleDown = 2.35f;
    [SerializeField] private float chargedJumpMinForce = 4f;
    [SerializeField] private float chargedJumpMaxForce = 20f;
    [SerializeField] private float chargedJumpMaxChargeTime = 2f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new(0f, -0.5f);
    [SerializeField] private Sprite crouchSprite;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private Transform interactionCircleTransform;
    [SerializeField] private float interactionZoneOffset = 0.5f;

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

    private float apexThreshold = 0.3f;

    private Vector2 moveInput;
    private float longRunTimer;
    private bool wantsToSprint;
    private bool wasGrounded;
    private bool isCrouched;
    private float currentSpeed;
    private Vector2 colliderSizeVelocity;
    private Vector2 colliderOffsetVelocity;

    private Animator animator;
    public int direction;
    private static readonly int xVelocity = Animator.StringToHash("xVelocity");

    private List<IInteractable> interactables = new List<IInteractable>();
    private const string GroundTag = "Ground";
    private const float MoveCancelChargeThreshold = 0.01f;

    private float chargeStartTime;
    private bool isChargingJump;

    private void CancelChargedJump()
    {
        isChargingJump = false;
    }

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
        animator = GetComponent<Animator>();

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
        if (isChargingJump && Mathf.Abs(move.x) > MoveCancelChargeThreshold)
            CancelChargedJump();

        direction = (int)move.x;
        if (direction != 0) spriteRenderer.flipX = direction < 0;
        animator.SetFloat(xVelocity, Math.Abs(move.x));
        moveInput = move;
    }

    private int FacingSide
    {
        get
        {
            return direction != 0
            ? direction : spriteRenderer != null && spriteRenderer.flipX ? -1 : 1;
        }
    }

    public void OnJumpPressed()
    {
        if (!IsGrounded()) return;

        if (isCrouched)
        {
            StartChargingJump();
        }
        else
        {
            ApplyNormalJump();
        }
    }

    public void OnJumpReleased()
    {
        if (isChargingJump)
            DoChargeJump();
    }

    private void ApplyNormalJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void StartChargingJump()
    {
        chargeStartTime = Time.time;
        isChargingJump = true;
    }

    private void DoChargeJump()
    {
        if (!isChargingJump) return;

        isChargingJump = false;

        float t = Mathf.Clamp(Time.time - chargeStartTime, 0f, chargedJumpMaxChargeTime);
        float progress = t / chargedJumpMaxChargeTime;
        float force = Mathf.Lerp(chargedJumpMinForce, chargedJumpMaxForce, progress);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        SetCrouching(false);
    }

    public void SetCrouching(bool crouch)
    {
        if (crouch && !IsGrounded()) return;

        if (!crouch && isChargingJump)
            CancelChargedJump();

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

    public void AddInteractable(IInteractable interactable)
    {
        interactables.Add(interactable);
    }

    public void RemoveInteractable(IInteractable interactable)
    {
        interactables.Remove(interactable);
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
        float closestDistance = float.MaxValue;
        IInteractable closestInteractable = null;
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i] == null) continue;

            float distance = Vector2.Distance(transform.position, interactables[i].Position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactables[i];
            }
        }
        if (closestInteractable != null)
        {
            closestInteractable.Interact(gameObject);
        }
    }

    private void UpdateInteractionZonePosition()
    {
        if (interactionCircleTransform == null) return;

        Vector3 localPos = interactionCircleTransform.localPosition;
        localPos.x = FacingSide * interactionZoneOffset;
        interactionCircleTransform.localPosition = localPos;
    }

    private void Update()
    {
        UpdateLongRunTimer();
        UpdateInteractionZonePosition();
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

        float targetVelX = moveInput.x * currentSpeed * (IsGrounded() ? 1f : airControlMultiplier);

        // При остановке принудительно обнуляем скорость, чтобы не было микродвижений и лишних перерасчётов физики
        if (Mathf.Abs(targetVelX) < 0.001f && Mathf.Abs(rb.linearVelocity.x) < velocityZeroThreshold)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            float lerpSpeed = IsGrounded() ? groundLerpSpeed : airLerpSpeed;
            float t = Mathf.Clamp01(lerpSpeed * Time.fixedDeltaTime);
            float newVelX = Mathf.Lerp(rb.linearVelocity.x, targetVelX, t);
            rb.linearVelocity = new Vector2(newVelX, rb.linearVelocity.y);
        }

        rb.gravityScale = (rb.linearVelocity.y < apexThreshold) ? gravityScaleDown : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 point = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(point, groundCheckRadius);
    }
}
