using UnityEngine;

public class FolowByX : MonoBehaviour
{
    [SerializeField] private Transform _followingTarget;
    [SerializeField] private float _additionalOffset;
    void Update()
    {
        transform.position = new Vector2(_followingTarget.position.x + _additionalOffset, transform.position.y);
    }
}
