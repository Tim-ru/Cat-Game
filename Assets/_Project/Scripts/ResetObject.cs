using UnityEngine;

public class ResetObject : MonoBehaviour
{
    private Vector3 _startPosition;
    void Start()
    {
        _startPosition = transform.position;
    }
    public void ResetObj()
    {
        gameObject.SetActive(false);
        transform.position = _startPosition;
    }
}
