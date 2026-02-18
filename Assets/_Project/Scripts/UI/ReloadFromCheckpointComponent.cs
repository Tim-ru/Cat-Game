using UnityEngine;
using UnityEngine.Events;

public class ReloadFromCheckpointComponent : MonoBehaviour
{
    [SerializeField] private CheckpointsComponent _checkpoints;
    [SerializeField] private UnityEvent _startEvent;
    public void ReloadScene()
    {
        _checkpoints.ReloadFromCheckpoint();
    }
    public void StartTheGame()
    {
        _startEvent?.Invoke();
    }
}
