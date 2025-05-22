using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetPlayerJumpDir()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }
}
