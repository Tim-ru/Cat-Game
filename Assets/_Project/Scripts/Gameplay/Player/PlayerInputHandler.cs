using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

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
        if (playerController != null)
        {
            playerController.OnJump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (playerController == null) return;

        if (context.canceled)
            playerController.SetCrouching(false);
        else if (context.started || context.performed)
            playerController.SetCrouching(true);
    }
}