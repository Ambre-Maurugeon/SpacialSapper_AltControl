using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields

    private bool _canReload;
    
    #endregion

    public static Vector2 MoveVector { get; private set; }
    public static Vector2 LookVector { get; private set; }
    public static event Action OnKeyboardReload ;
    public static event Action OnArduinoReload ;

    public void ReadMoveInput(InputAction.CallbackContext context)
    {
        MoveVector = context.ReadValue<Vector2>();
    }

    public void ReadLookInput(InputAction.CallbackContext context)
    {
        LookVector = context.ReadValue<Vector2>();
    }

    public void ReadReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;  

        if( _canReload)
        {
            _canReload = false;

            Debug.Log("Reload");
            OnKeyboardReload?.Invoke();
        }
    }

    public void ReadCanReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        _canReload = true;
        Debug.Log("Can reload.");
    }

    public static void ReloadFromArduino()
    {
        OnArduinoReload?.Invoke();
    }
}
