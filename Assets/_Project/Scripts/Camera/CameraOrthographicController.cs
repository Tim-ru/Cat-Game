using UnityEngine;
using Unity.Cinemachine;

public class CameraOrthographicController : MonoBehaviour
{
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float transitionSpeed = 3f;

    private float defaultSize;
    private float targetSize;
    private float currentSize;

    void Start()
    {
        if (cinemachineCamera == null)
            cinemachineCamera = GetComponent<CinemachineCamera>();

        defaultSize = cinemachineCamera.Lens.OrthographicSize;
        targetSize = defaultSize;
        currentSize = defaultSize;
    }

    void Update()
    {
        if (cinemachineCamera == null) return;

        currentSize = Mathf.Lerp(currentSize, targetSize, transitionSpeed * Time.deltaTime);

        var lens = cinemachineCamera.Lens;
        lens.OrthographicSize = currentSize;
        cinemachineCamera.Lens = lens;
    }

    public void SetTargetOrthographicSize(float size)
    {
        targetSize = size;
    }

    public void ResetToDefault()
    {
        targetSize = defaultSize;
    }
}
