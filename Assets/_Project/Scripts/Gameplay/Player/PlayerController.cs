using System;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float chargedJumpAngleFromNormal = 15f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Vector2 groundCheckOffset = new(0f, -0.5f);
    [SerializeField] private Sprite crouchSprite;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private Transform interactionCircleTransform;
    [SerializeField] private float interactionZoneOffset = 0.5f;

    [SerializeField] private ParticleSystem runParticles;
    [SerializeField] private PlayerVFXSpawner vfxSpawner;

    private float crouchSpeed = 1.5f;
    private float colliderTransitionSpeed = 6f;
    private Vector2 crouchColliderSize = new(0.44f, 0.25f);
    private Vector2 crouchColliderOffset = new(-0.03f, -0.525f);
    private Vector2 standColliderSize = new(0.5f, 0.4f);
    private Vector2 standColliderOffset = new(0.05f, -0.45f);

    [SerializeField] private float slowRunSpeed = 10f;
    [SerializeField] private float chaseRunSpeed = 15f;
    private float longRunDurationToActivate = 3f;
    private float longRunMultiplier = 1.4f;
    private float longRunMinMoveInput = 0.1f;

    private float apexThreshold = 0.3f;

    private Vector2 moveInput;
    private float longRunTimer;
    private bool wantsToSprint;
    private bool wasGrounded;
    private bool isCrouching;
    private bool crouchPressed;
    private float currentSpeed;
    private Vector2 colliderSizeVelocity;
    private Vector2 colliderOffsetVelocity;

    private Animator animator;
    public int direction;
    private static readonly int xVelocity = Animator.StringToHash("xVelocity");
    private static readonly int yVelocity = Animator.StringToHash("yVelocity");
    private static readonly int isSprintingParam = Animator.StringToHash("isSprinting");
    private static readonly int isChaseParam = Animator.StringToHash("isChase");
    private static readonly int strongJumpParam = Animator.StringToHash("StrongJump");

    private readonly float strongJumpHoldAtNormalizedTime = 0.7f;

    private const string StrongJumpStateName = "strongJump";
    private const int BaseLayerIndex = 0;

    private List<IInteractable> interactables = new List<IInteractable>();
    private const string GroundTag = "Ground";

    private float chargeStartTime;
    private bool isChargingJump;
    private bool isChase;
    private Vector2 lastAirVelocity;

    private Vector2 _pointLeft;
    private Vector2 _pointRight;

    public void SetChase(bool chase)
    {
        if (isChase == chase) return;
        isChase = chase;
        if (IsGrounded())
            ApplyMovementSpeed();
    }

    private void CancelChargedJump()
    {
        isChargingJump = false;
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
            if (stateInfo.IsName(StrongJumpStateName))
                animator.Play("idle", BaseLayerIndex);
        }
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
        if (vfxSpawner == null) vfxSpawner = GetComponent<PlayerVFXSpawner>();
        animator = GetComponent<Animator>();

        currentSpeed = maxSpeed;
        if (boxCollider != null)
        {
            boxCollider.size = standColliderSize;
            boxCollider.offset = standColliderOffset;
        }
        SetCrouching(false);
        wasGrounded = IsGrounded();
        _pointRight = (Vector2)boxCollider.bounds.center;
        _pointRight.x -= standColliderSize.x / 2;
        _pointRight.y += crouchColliderSize.y / 2;
        _pointLeft = (Vector2)boxCollider.bounds.center;
        _pointLeft.x += standColliderSize.x / 2;
        _pointLeft.y += crouchColliderSize.y / 2;
    }

    public void OnMove(Vector2 move)
    {
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

        if (isCrouching)
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
        if (vfxSpawner != null)
            vfxSpawner.SpawnJumpTakeoff(GetFeetPosition(), jumpForce);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void StartChargingJump()
    {
        chargeStartTime = Time.time;
        isChargingJump = true;
        animator.SetTrigger(strongJumpParam);
    }

    private void DoChargeJump()
    {
        if (!isChargingJump) return;

        isChargingJump = false;

        float t = Mathf.Clamp(Time.time - chargeStartTime, 0f, chargedJumpMaxChargeTime);
        float progress = t / chargedJumpMaxChargeTime;
        float force = Mathf.Lerp(chargedJumpMinForce, chargedJumpMaxForce, progress);

        float angleRad = chargedJumpAngleFromNormal * Mathf.Deg2Rad;
        Vector2 jumpDirection = new Vector2(FacingSide * Mathf.Sin(angleRad), Mathf.Cos(angleRad));
        if (vfxSpawner != null)
            vfxSpawner.SpawnJumpTakeoff(GetFeetPosition(), force);
        rb.linearVelocity = jumpDirection * force;
        SetCrouching(false);
    }

    public void SetCrouching(bool crouch)
    {
        if (!crouch && isChargingJump)
            CancelChargedJump();
        crouchPressed = crouch;
        if (!crouch)
        {
            _pointRight = (Vector2)boxCollider.bounds.center;
            _pointRight.x -= standColliderSize.x / 2;
            _pointRight.y += crouchColliderSize.y / 2;
            bool _isAbleToStandUp;
            var hit1 = Physics2D.RaycastAll(_pointRight, Vector2.up, 0.15f);
            _isAbleToStandUp = !CheckGround(hit1);
            if (!_isAbleToStandUp) return;
            _pointLeft = (Vector2)boxCollider.bounds.center;
            _pointLeft.x += standColliderSize.x / 2;
            _pointLeft.y += crouchColliderSize.y / 2;
            var hit2 = Physics2D.RaycastAll(_pointLeft, Vector2.up, 0.15f);
            _isAbleToStandUp = !CheckGround(hit2);
            if (!_isAbleToStandUp) return;
        }
        isCrouching = crouch;

        if (spriteRenderer != null)
        {
            Sprite newSprite = crouch ? crouchSprite : idleSprite;
            if (newSprite != null)
                spriteRenderer.sprite = newSprite;
#if UNITY_EDITOR
            Debug.Log($"[Crouch] SetCrouching({crouch}), sprite: {(newSprite != null ? newSprite.name : "NULL")}");
#endif
        }

        ApplyMovementSpeed();
    }

    private bool CheckGround(RaycastHit2D[] hit)
    {
        if (hit.Length != 0)
        {
            foreach (var ht in hit)
            {
                if (ht.collider.CompareTag(GroundTag)) return true;
            }
            return false;
        }
        else return false;
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

        bool isLongRunActive = longRunTimer >= longRunDurationToActivate && !isCrouching;

        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (wantsToSprint)
            currentSpeed = isChase ? chaseRunSpeed : slowRunSpeed;
        else if (isLongRunActive)
            currentSpeed = maxSpeed * longRunMultiplier;
        else
            currentSpeed = maxSpeed;

    }

    private Vector3 GetFeetPosition()
    {
        return transform.position + (Vector3)groundCheckOffset;
    }

    private void UpdateRunParticles()
    {
        if (runParticles == null) return;
        bool shouldRun = IsGrounded() && Mathf.Abs(moveInput.x) >= 0.05f && currentSpeed > maxSpeed;
        if (shouldRun)
        {
            runParticles.gameObject.SetActive(true);
            Vector3 scale = runParticles.transform.localScale;
            scale.x = -Mathf.Sign(moveInput.x);
            runParticles.transform.localScale = scale;
            if (!runParticles.isPlaying)
                runParticles.Play();
        }
        else
        {
            if (runParticles.isPlaying)
                runParticles.Stop();
            runParticles.gameObject.SetActive(false);
        }
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

        if (!isCrouching && isMoving)
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
        UpdateStrongJumpAnimationHold();
        UpdateRunParticles();
    }

    private void UpdateStrongJumpAnimationHold()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        bool isInStrongJumpState = stateInfo.IsName(StrongJumpStateName);

        if (!isInStrongJumpState)
        {
            animator.speed = 1f;
            return;
        }

        bool shouldHoldAtFrame6 = isChargingJump && IsGrounded() && stateInfo.normalizedTime >= strongJumpHoldAtNormalizedTime;

        if (shouldHoldAtFrame6)
        {
            animator.speed = 0f;
            animator.Play(Animator.StringToHash(StrongJumpStateName), BaseLayerIndex, strongJumpHoldAtNormalizedTime);
        }
        else
        {
            animator.speed = 1f;
        }
    }

    private void LateUpdate()
    {
        // Не перезаписываем спрайт при зарядке прыжка — анимация strong_jump сама меняет кадры
        if (isCrouching && !isChargingJump && spriteRenderer != null && crouchSprite != null)
            spriteRenderer.sprite = crouchSprite;
    }


    private void FixedUpdate()
    {
        float speedForAnimator = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat(xVelocity, speedForAnimator);
        animator.SetFloat(yVelocity, rb.linearVelocityY);
        animator.SetBool(isSprintingParam, wantsToSprint);
        animator.SetBool(isChaseParam, isChase);
        bool grounded = IsGrounded();
        if (!wasGrounded && grounded)
        {
            ApplyMovementSpeed();
            if (vfxSpawner != null)
                vfxSpawner.SpawnLanding(GetFeetPosition(), lastAirVelocity.y);
        }
        if (!grounded)
            lastAirVelocity = rb.linearVelocity;
        wasGrounded = grounded;

        if (boxCollider != null)
        {
            Vector2 targetSize = isCrouching ? crouchColliderSize : standColliderSize;
            Vector2 targetOffset = isCrouching ? crouchColliderOffset : standColliderOffset;
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

        if (crouchPressed != isCrouching)
        {
            SetCrouching(crouchPressed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 point = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(point, groundCheckRadius);
        Gizmos.DrawLine(_pointRight, _pointRight + Vector2.up * 0.15f);
        Gizmos.DrawLine(_pointLeft, _pointLeft + Vector2.up * 0.15f);
    }

    [ContextMenu("Change")]
    private void ChangeState()
    {
        isCrouching = !isCrouching;
        if (isCrouching)
        {
            boxCollider.size = crouchColliderSize;
            boxCollider.offset = crouchColliderOffset;
        }
        else
        {
            boxCollider.size = standColliderSize;
            boxCollider.offset = standColliderOffset;
        }
    }
}
