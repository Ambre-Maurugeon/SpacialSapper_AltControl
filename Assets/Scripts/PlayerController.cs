using System;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields
    private static Vector2 _moveVector;
    private static Vector2 _lookVector;

    private static bool _canReload;

    #endregion
    public static Vector2 MoveVector => _moveVector;
    public static Vector2 LookVector => _lookVector;

    public static event Action OnReload ;

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector2>();
    }

    public void ReadLookInput(InputAction.CallbackContext context)
    {
        _lookVector = context.ReadValue<Vector2>();
    }

    public void ReadReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;  

        if( _canReload)
        {
            _canReload = false;

            Debug.Log("Reload");
            OnReload?.Invoke();
        }
    }

    public void ReadCanReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        _canReload = true;
        Debug.Log("Can reload.");
    }
}
