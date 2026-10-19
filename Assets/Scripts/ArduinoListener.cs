using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class ArduinoListener : MonoBehaviour
{
    
    [SerializeField] private int _centerVerticalDeadZoneRange;
    [SerializeField] private int _centerHorizontalDeadZoneRange;
    [SerializeField] private int _borderVerticalDeadZoneRange;
    [SerializeField] private int _borderHorizontalDeadZoneRange;
    [SerializeField] Slider _sliderYaw;
    [SerializeField] Slider _sliderPitch;
    [SerializeField] Slider _sliderEncoder1;
    [SerializeField] Slider _sliderEncoder2;

    
    private bool IsInDeadZone = false;
    private void Start()
    {
        _sliderYaw.minValue = 0;
        _sliderYaw.maxValue = 90;
        _sliderPitch.minValue = 0;
        _sliderPitch.maxValue = 30;
    }
    
    private float[] CheckForDeadZone(float yaw, float pitch)
    {
        IsInDeadZone = false;
        // Center Vertical Zone
        if (((_sliderYaw.maxValue / 2 - _centerVerticalDeadZoneRange / 2) < yaw) 
            && (yaw < (_sliderYaw.maxValue / 2 + _centerVerticalDeadZoneRange / 2)))
        {
            //Debug.Log("Center vertical");
            yaw = _sliderYaw.maxValue / 2;
            IsInDeadZone = true;
        }
        
        // Center Horizontal Zone
        if (((_sliderPitch.maxValue / 2 - _centerHorizontalDeadZoneRange / 2) < pitch) 
            && (pitch < (_sliderPitch.maxValue / 2 + _centerHorizontalDeadZoneRange / 2)))
        {
            //Debug.Log("Center horizontal");
            //pitch = Lerp(pitch, _sliderPitch.maxValue / 2, .5f);
            pitch = _sliderPitch.maxValue / 2;
            IsInDeadZone = true;
        }
        
        // Border Vertical Zone
        if ((_sliderYaw.maxValue - _borderVerticalDeadZoneRange) < yaw)
        {
            //Debug.Log("Border vertical");
            IsInDeadZone = true;
            yaw = _sliderYaw.maxValue;
        }
        if ((_sliderYaw.minValue + _borderVerticalDeadZoneRange) > yaw)
        {
            //Debug.Log("Border vertical");
            IsInDeadZone = true;
            yaw = _sliderYaw.minValue;
        }
        
        //Border Horizontal Zone
        if ((_sliderPitch.maxValue - _borderHorizontalDeadZoneRange) < pitch)
        {
            //Debug.Log("Border horizontal");
            IsInDeadZone = true;
            pitch = _sliderPitch.maxValue;
        }
        if ((_sliderPitch.minValue + _borderHorizontalDeadZoneRange) > pitch)
        {
            //Debug.Log("Border horizontal");
            IsInDeadZone = true;
            pitch = _sliderPitch.minValue;
        }
        return new float[] { yaw, pitch };
    }
    public void OnMessageArrived(string message)
    {
        var splitMessage = message.Split(':');
        if (splitMessage.Length is < 2) return;
        //Debug.Log(splitMessage[0]); // se clamp entre 230 et 320 ???
        float yawValue = int.Parse(splitMessage[0]);
        float pitchValue = int.Parse(splitMessage[1]);
        //if (yawValue >= 180) yawValue = 0f;
        yawValue = CheckForDeadZone(yawValue, pitchValue)[0];
        pitchValue = CheckForDeadZone(yawValue, pitchValue)[1];
        yawValue = Clamp(yawValue, _sliderYaw.minValue, _sliderYaw.maxValue);
        pitchValue = Clamp(pitchValue, _sliderPitch.minValue, _sliderPitch.maxValue);
        Debug.Log("yaw : " + yawValue + "  |  pitch : " + pitchValue);
        _sliderYaw.value = yawValue;
        _sliderPitch.value = pitchValue;
        if (splitMessage.Length > 2)
        {
            var Encoder1ValueModifier = splitMessage[2];
            if (Encoder1ValueModifier == "+") _sliderEncoder1.value++; 
            else if (Encoder1ValueModifier == "-") _sliderEncoder1.value--; 
            //Debug.Log("EncoderValueModifier : " + Encoder1ValueModifier);
            var Encoder2ValueModifier = splitMessage[3];
            if (Encoder2ValueModifier == "+") _sliderEncoder2.value++; 
            else if (Encoder2ValueModifier == "-") _sliderEncoder2.value--; 
            //Debug.Log("EncoderValueModifier : " + Encoder1ValueModifier);
        }
    }
    
    
    public void OnConnectionEvent(bool useless) {}
    
    
}
