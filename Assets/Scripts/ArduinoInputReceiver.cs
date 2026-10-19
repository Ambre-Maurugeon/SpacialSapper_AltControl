using System;
using UnityEngine;
using static UnityEngine.Mathf;

public class ArduinoInputReceiver : MonoBehaviour
{
    public static ArduinoInputReceiver Instance {get; private set;}

    public float _yaw {get; private set;}
    public float _pitch {get; private set;}
    
    public ENCODERTYPE EncoderValue {get; private set;}
    
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
        var splitMessage = message.Split(':');
        if (splitMessage.Length != 3) return;
        _yaw = int.Parse(splitMessage[0]);
        _pitch = int.Parse(splitMessage[1]);
        if (_yaw >= 180) _yaw = 0f;
        _yaw -= 45;
        _pitch -= 15;
        switch (splitMessage[2])
        {
            case "+":
                EncoderValue = ENCODERTYPE.POSITIVE;
                break;
            case "-":
                EncoderValue = ENCODERTYPE.NEGATIVE;
                break;
            case "=":
                EncoderValue = ENCODERTYPE.NEUTRAL;
                break;
            default:
                EncoderValue = ENCODERTYPE.NEUTRAL;
                break;
         }

    }
    
    public void OnConnectionEvent(bool useless) {}
    
}
