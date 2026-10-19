using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WavesSO", menuName = "Scriptable Objects/WavesSO")]
public class WavesSO : ScriptableObject
{
    public List<WaveData> waves = new();

    public WaveData GetData(int index) => waves[index];
}
