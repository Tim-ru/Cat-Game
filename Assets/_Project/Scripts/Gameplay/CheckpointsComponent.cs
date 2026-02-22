using UnityEngine;
using UnityEngine.Events;

public class CheckpointsComponent : MonoBehaviour
{
    [SerializeField] private Vector3 _lastCheckpointPosition;
    [SerializeField] private GameObject _player;
    [SerializeField] private UnityEvent _reloadedEvent;
    private void Start()
    {
        _lastCheckpointPosition = _player.transform.position;
    }
    public void UpdateCheckpoint(Vector2 _spawnPosition)
    {
        _lastCheckpointPosition = _spawnPosition;
    }

    public void ReloadFromCheckpoint()
    {
        _player.transform.position = _lastCheckpointPosition;
        _reloadedEvent?.Invoke();
    }
}
