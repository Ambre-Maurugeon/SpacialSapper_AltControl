using System;
using UnityEngine;
using static UnityEngine.Mathf;

public class ArduinoInputReceiver : MonoBehaviour
{
    public static ArduinoInputReceiver Instance {get; private set;}

    public float _yaw {get; private set;}
    public float _pitch {get; private set;}
    
    public ENCODERTYPE Encoder1Value {get; private set;}
    public ENCODERTYPE Encoder2Value {get; private set;}
    
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
        if (splitMessage.Length < 2) return;
        _yaw = int.Parse(splitMessage[0]);
        _pitch = int.Parse(splitMessage[1]);
        if (_yaw >= 180) _yaw = 0f;
        _yaw -= 45;
        _pitch -= 15;
        if (splitMessage.Length != 4) return;
        switch (splitMessage[2])
        {
            case "+":
                Encoder1Value = ENCODERTYPE.POSITIVE;
                break;
            case "-":
                Debug.Log("Encoder 1 Negative");
                Encoder1Value = ENCODERTYPE.NEGATIVE;
                break;
            case "=":
                Encoder1Value = ENCODERTYPE.NEUTRAL;
                break;
            default:
                Encoder1Value = ENCODERTYPE.NEUTRAL;
                break;
        }
        switch (splitMessage[3])
        {
            case "+":
                Encoder2Value = ENCODERTYPE.POSITIVE;
                break;
            case "-":
                Encoder2Value = ENCODERTYPE.NEGATIVE;
                break;
            case "=":
                Encoder2Value = ENCODERTYPE.NEUTRAL;
                break;
            default:
                Encoder2Value = ENCODERTYPE.NEUTRAL;
                break;
        }

    }
    
    public void OnConnectionEvent(bool useless) {}
    
}
