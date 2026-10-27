using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields
    [Header("Reload")]
    [SerializeField] private float _simultaneousTolerance = 0.2f;

    private bool _reloadFirst = false;
    private bool _reloadSecond = false;

    #endregion

    public static Vector2 MoveVector { get; private set; }
    public static Vector2 ArduinoMoveVector { get; private set; }
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

    #region Reload
    public void ReadFirstReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        StartCoroutine("StartFirstCooldown");
        if (_reloadSecond) OnKeyboardReload?.Invoke(); 


    }

    public void ReadSecondReloadInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        StartCoroutine("StartSecondCooldown");
        if (_reloadFirst) OnKeyboardReload?.Invoke();
    }

    public static void ReloadFromArduino()
    {
        OnArduinoReload?.Invoke();
    }

    private IEnumerator StartFirstCooldown()
    {
        _reloadFirst = true;
        yield return new WaitForSeconds(_simultaneousTolerance);
        _reloadFirst = false;

    }

    private IEnumerator StartSecondCooldown()
    {
        _reloadSecond = true;
        yield return new WaitForSeconds(_simultaneousTolerance);
        _reloadSecond = false;
    }

    #endregion 

    public static void ReadArduinoEncoderInput()
    {
        ArduinoMoveVector = new Vector2((int)ArduinoInputReceiver.Instance._encoder1Value,  (int)ArduinoInputReceiver.Instance._encoder2Value);
    }


}
