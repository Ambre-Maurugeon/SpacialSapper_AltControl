using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class ArduinoListener : MonoBehaviour
{

    [Header("Valeurs d'équilibrage")] 
    [Space(10)]
    
    [InfoBox("Valeurs en ° des zones mortes")]
    [Space(10)]
    
    [Tooltip("Angle de la zone morte verticale au centre de l'écran (en °)")]
    [SerializeField] private int _centerVerticalDeadZoneRange;
    [Space(5)]
    [Tooltip("Angle de la zone morte horizontale au centre de l'écran (en °)")]
    [SerializeField] private int _centerHorizontalDeadZoneRange;
    [Space(5)]
    [Tooltip("Angle de la zone morte verticale aux bords de l'écran (en °)")]
    [SerializeField] private int _borderVerticalDeadZoneRange;
    [Space(5)]
    [Tooltip("Angle de la zone morte horizontales aux bords de l'écran (en °)")]
    [SerializeField] private int _borderHorizontalDeadZoneRange;
    [Foldout("References")]
    [SerializeField] Slider _sliderYaw;
    [Foldout("References")] 
    [SerializeField] Slider _sliderPitch;

    
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
    }
    
    
    public void OnConnectionEvent(bool useless) {}
    
    
}
