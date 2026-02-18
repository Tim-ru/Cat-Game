using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] float orthographicSize = 4f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var cam = FindObjectOfType<CameraOrthographicController>();
        if (cam != null)
        {
            cam.SetTargetOrthographicSize(orthographicSize);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var cam = FindObjectOfType<CameraOrthographicController>();
        if (cam != null)
        {
            cam.ResetToDefault();

        }
    }
}
