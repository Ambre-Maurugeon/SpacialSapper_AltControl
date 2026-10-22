using System;
using UnityEngine;
using static UnityEngine.Mathf;

public class ArduinoInputReceiver : MonoBehaviour
{
#region Fields
    public static ArduinoInputReceiver Instance {get; private set;}
    public float _yaw {get; private set;}
    public float _pitch {get; private set;}
    public ENCODERTYPE _encoder1Value {get; private set;}
    public ENCODERTYPE _encoder2Value {get; private set;}
    
    private bool _isButtonOn = false;
    private bool _wasButtonOn = false;

    [SerializeField] private float _encoderTimer; // same as Arduino ----------------------------------------------------------------------------
    private float _lastEncoderTimer;
    
#endregion

#region DebugTools

    public bool _useKeyboard;

#endregion
    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    

    
    public void OnMessageArrived(string message)
    {
        //Debug.Log(message);
        var splitMessage = message.Split(':');
        if (splitMessage.Length < 2) return;
        _isButtonOn = splitMessage[^1] == "1";
        _yaw = int.Parse(splitMessage[0]);
        _pitch = int.Parse(splitMessage[1]);
        if (_yaw >= 180) _yaw = 0f;
        _yaw -= 45;
        _pitch -= 15;
        if (splitMessage.Length != 5) return;
        switch (splitMessage[2])
        {
            case "+":
                _encoder1Value = ENCODERTYPE.POSITIVE;
                break;
            case "-":
                _encoder1Value = ENCODERTYPE.NEGATIVE;
                break;
            case "=":
                _encoder1Value = ENCODERTYPE.NEUTRAL;
                break;
            default:
                _encoder1Value = ENCODERTYPE.NEUTRAL;
                break;
        }
        switch (splitMessage[3])
        {
            case "+":
                _encoder2Value = ENCODERTYPE.POSITIVE;
                break;
            case "-":
                _encoder2Value = ENCODERTYPE.NEGATIVE;
                break;
            case "=":
                _encoder2Value = ENCODERTYPE.NEUTRAL;
                break;
            default:
                _encoder2Value = ENCODERTYPE.NEUTRAL;
                break;
        }

        if (Time.time - _lastEncoderTimer >= _encoderTimer)
        {
            //Debug.Log("Receiving Encoders : " + _encoder1Value + ":" + _encoder2Value);
            _lastEncoderTimer = Time.time;
            PlayerController.ReadArduinoEncoderInput();
        }

        if (_isButtonOn && !_wasButtonOn)
        {
            //Debug.Log("Reloading from Arduino");
            PlayerController.ReloadFromArduino();
        }

        _wasButtonOn = _isButtonOn;
    }
    
    public void OnConnectionEvent(bool useless) {}
    
}
