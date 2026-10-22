using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin basicMultiChannelPerlin;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        basicMultiChannelPerlin = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float duration)
    {
        // set shake
        basicMultiChannelPerlin.AmplitudeGain = intensity;

        // reset
        Invoke("ResetShake",duration);

    }

    private void ResetShake()
    {
        basicMultiChannelPerlin.AmplitudeGain = 0f;
    }


}
