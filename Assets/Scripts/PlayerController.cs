using System;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields

    private bool _canReload;

    #endregion

    public static Vector2 MoveVector { get; private set; }
    public static Vector2 LookVector { get; private set; }
    
    public static Vector2 FaucetInput { get; private set; }

    public static event Action OnReload ;

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
            OnReload?.Invoke();
        }
    }

    public void ReadCanReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        _canReload = true;
        Debug.Log("Can reload.");
    }
    
    public void OnMessageArrived(string message)
    {
        var splitMessage = message.Split(':');
        float yawValue = int.Parse(splitMessage[0]);
        float pitchValue = int.Parse(splitMessage[1]);
        Debug.Log("yaw : " + yawValue + "  |  pitch : " + pitchValue);
        FaucetInput = new Vector2(yawValue, pitchValue);
    }
    
    
    public void OnConnectionEvent(bool useless) {}

    
}
