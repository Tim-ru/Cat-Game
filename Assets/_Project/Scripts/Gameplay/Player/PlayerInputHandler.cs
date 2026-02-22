using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject _canvas;
    private PlayerInput _playerInput;
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();

        if (playerController != null)
        {
            playerController.OnMove(move);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (playerController == null) return;

        if (context.started)
            playerController.OnJumpPressed();
        else if (context.canceled)
            playerController.OnJumpReleased();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (playerController == null) return;

        if (context.canceled)
            playerController.SetCrouching(false);
        else if (context.started || context.performed)
            playerController.SetCrouching(true);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (playerController == null) return;

        if (context.canceled)
            playerController.SetSprinting(false);
        else if (context.started || context.performed)
            playerController.SetSprinting(true);

    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Time.timeScale = 0;
            _playerInput.SwitchCurrentActionMap("UI");
            _canvas.SetActive(true);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerController.OnInteract();
        }
    }
}