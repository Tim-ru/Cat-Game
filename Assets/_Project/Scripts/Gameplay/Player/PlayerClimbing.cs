using System;
using System.Collections;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    [SerializeField] private Vector2 _cornerCheckOffset = new(0.3f, 0.2f);
    [SerializeField] private float _checkDistance = 0.5f;
    [SerializeField] private float _climbingTime = 1f;
    [Range(0, 1f)]
    [SerializeField] private float _heightClimbingTimePercentage = 0.5f;
    private const string Corner = "Corner";
    private Vector2 _point;
    public bool _isTouching = false;
    private Coroutine _climbingCoroutine;
    [SerializeField] int _direction;
    private Rigidbody2D _rd;

    private void Start()
    {
        _rd = GetComponent<Rigidbody2D>();   
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        _point = new Vector2(transform.position.x, transform.position.y) + _cornerCheckOffset;
        _cornerCheckOffset.x *= _direction;
    }
#endif
    private void Update()
    {
        _point = (Vector2)transform.position + _cornerCheckOffset * _direction;
        RaycastHit2D[] hits = Physics2D.RaycastAll(_point, Vector2.right * _direction, _checkDistance);
        foreach (RaycastHit2D col in hits)
        {
            if (col.collider.CompareTag(Corner))
            {
                if (_climbingCoroutine == null)
                {
                    _rd.linearVelocity = Vector2.zero;
                    StartCoroutine(StartClimbing(col));
                }
                return;
            }
        }
        _isTouching =  false;
    }

    private IEnumerator StartClimbing(RaycastHit2D col)
    {
        var startPos = transform.position;
        int tickCount = (int)(_climbingTime * 1f / Time.fixedDeltaTime);
        float yTicks = (float)Math.Floor(tickCount * _heightClimbingTimePercentage);
        float xTicks = tickCount - yTicks;
        for (int i = 0; i <= yTicks; i++)
        {
            var newYPos = Mathf.Lerp(startPos.y, col.point.y + 1, i / yTicks);
            transform.position = new Vector2(startPos.x, newYPos);
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i <= xTicks; i++)
        {
            var newXPos = Mathf.Lerp(startPos.x, col.point.x, i / xTicks);
            transform.position = new Vector2(newXPos, col.point.y + 1);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isTouching ? Color.green : Color.red;
        Gizmos.DrawLine(_point, _point + _checkDistance * _direction * Vector2.right);
    }
}
