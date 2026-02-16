using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    [SerializeField] private MenuNavigationComponent _menu;
    public delegate void OnNavigationChanged(float y);
    public event OnNavigationChanged OnNavigation;
    public delegate void OnSubmitPressed();
    public event OnSubmitPressed OnSubmit;
    public void Navigation(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        OnNavigation(move.y);
    }

    public void Submit(InputAction.CallbackContext context)
    {
        OnSubmit();
    }
    public void Cancel(InputAction.CallbackContext context)
    {
        _menu.Back();
    }
}
