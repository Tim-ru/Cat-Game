using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    private MenuNavigationComponent _menu;
    public delegate void OnNavigationChanged(Vector2 vector);
    public event OnNavigationChanged OnNavigation;
    public delegate void OnSubmitPressed();
    public event OnSubmitPressed OnSubmit;

    private void Start()
    {
        _menu = GetComponent<MenuNavigationComponent>();
    }
    public void Navigation(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Vector2 move = context.ReadValue<Vector2>();
        OnNavigation(move);
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        OnSubmit();
    }
    public void Cancel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _menu.Back();
    }
}
