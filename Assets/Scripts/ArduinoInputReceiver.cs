using System;
using UnityEngine;
using static UnityEngine.Mathf;

public class ArduinoInputReceiver : MonoBehaviour
{
    public static ArduinoInputReceiver Instance {get; private set;}

    public float _yaw {get; private set;}
    public float _pitch {get; private set;}
    
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
        Debug.Log(message);
        var splitMessage = message.Split(':');
        //Debug.Log(splitMessage[0]); // se clamp entre 230 et 320 ???
        _yaw = int.Parse(splitMessage[0]);
        _pitch = int.Parse(splitMessage[1]);
        if (_yaw >= 180) _yaw = 0f;
        _yaw -= 45;
        _pitch -= 15;
        // _yaw = CheckForDeadZone(_yaw, _pitch)[0];
        // _pitch = CheckForDeadZone(_yaw, _pitch)[1];
        // _yaw = Clamp(_yaw, ); // Clamp with screen size
        // _pitch = Clamp(_pitch, ); // Clamp with screen size
        //Debug.Log("yaw : " + _yaw + "  |  pitch : " + _pitch);
        // _sliderYaw.value = _yaw;
        // _sliderPitch.value = _pitch;
    }
    
    public void OnConnectionEvent(bool useless) {}
    
}
