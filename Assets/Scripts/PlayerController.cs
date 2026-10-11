using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static Vector2 _moveVector;
    private static Vector2 _lookVector;
    public static Vector2 MoveVector => _moveVector;
    public static Vector2 LookVector => _lookVector;

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }

    public void ReadLookInput(InputAction.CallbackContext context)
    {
        _lookVector = context.ReadValue<Vector2>();
    }
}
