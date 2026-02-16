using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        Debug.Log($"OnMove called, value: {move}");

        if (playerController != null)
        {
            Debug.Log($"OnMove called, controller is null: {playerController == null}, value: {context.ReadValue<Vector2>()}");
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
}