using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StartTheGameComponent : MonoBehaviour
{
    [SerializeField] private UnityEvent _buttonPressedEvent;
    [SerializeField] private UnityEvent _startEvent;
    private void Start()
    {
        _startEvent?.Invoke();
        InputSystem.onAnyButtonPress
    .CallOnce(ctrl => _buttonPressedEvent?.Invoke());

    }
}
