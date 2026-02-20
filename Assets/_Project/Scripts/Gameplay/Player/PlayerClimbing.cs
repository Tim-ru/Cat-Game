using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbing : MonoBehaviour
{
    [SerializeField] private Vector2 _cornerCheckOffset = new(0.6f, -0.4f);
    [SerializeField] private float _checkDistance = 0.5f;
    [SerializeField] private float _climbingTime = 1f;
    [Range(0, 1f)]
    [SerializeField] private float _heightClimbingTimePercentage = 0.5f;
    [SerializeField] private float _climbingHeight = 1f;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerController _playerController;
    private const string Ground = "Ground";
    private Vector2 _point = Vector2.zero;
    private Coroutine _climbingCoroutine;
    private Rigidbody2D _rd;
    private Animator _animator;
    private readonly static int CLimbing = Animator.StringToHash("Climbing");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rd = GetComponent<Rigidbody2D>();
        if (!_playerController) _playerController = GetComponent<PlayerController>();
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        _point = new Vector2(transform.position.x, transform.position.y) + _cornerCheckOffset;
    }
#endif
    private void Update()
    {
        _point = (Vector2)transform.position;
        _point.x += _cornerCheckOffset.x * _playerController.direction;
        _point.y += _cornerCheckOffset.y;
        RaycastHit2D[] hits = Physics2D.RaycastAll(_point, Vector2.right * _playerController.direction, _checkDistance);
        foreach (RaycastHit2D col in hits)
        {
            if (col.collider.CompareTag(Ground))
            {
                var hitPoint = col.point;
                hitPoint.x += 0.3f * _playerController.direction;
                hitPoint.y += _climbingHeight * 1.5f;
                RaycastHit2D upperObject = Physics2D.Raycast(hitPoint, Vector2.down, 1.5f);
                var distance = Math.Abs(upperObject.point.y - hitPoint.y);
                if (distance >= _climbingHeight && _climbingCoroutine == null)
                {
                    var nextPos = hitPoint;
                    nextPos.y -= (distance - _climbingHeight) + _cornerCheckOffset.y;
                    nextPos.y -= _climbingHeight / 2;
                    nextPos.y -= 0.3f;
                    var startAnimPos = transform.position;
                    startAnimPos.x = col.point.x - 0.2f * _playerController.direction;
                    startAnimPos.y = nextPos.y - 1f;
                    transform.position = startAnimPos;
                    _rd.bodyType = RigidbodyType2D.Kinematic;
                    _rd.linearVelocity = Vector2.zero;
                    _animator.SetTrigger(CLimbing);
                    _playerInput.enabled = false;
                    _climbingCoroutine = StartCoroutine(StartClimbing(nextPos));
                }
                return;
            }
        }
    }

    private IEnumerator StartClimbing(Vector2 target)
    {
        var startPos = transform.position;
        int tickCount = (int)(_climbingTime * 1f / Time.fixedDeltaTime);
        float yTicks = (float)Math.Floor(tickCount * _heightClimbingTimePercentage);
        float xTicks = tickCount - yTicks;
        for (int i = 0; i <= yTicks; i++)
        {
            var newYPos = Mathf.Lerp(startPos.y, target.y, i / yTicks);
            _rd.MovePosition(new Vector2(startPos.x, newYPos));
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i <= xTicks; i++)
        {
            var newXPos = Mathf.Lerp(startPos.x, target.x, i / xTicks);
            _rd.MovePosition(new Vector2(newXPos, target.y));
            yield return new WaitForFixedUpdate();
        }
        _rd.bodyType = RigidbodyType2D.Dynamic;
        _climbingCoroutine = null;
        _playerInput.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(_point, _point + _checkDistance * (Vector2.right * _playerController.direction));
    }
}
