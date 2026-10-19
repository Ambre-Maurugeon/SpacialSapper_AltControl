using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WavesSO", menuName = "Scriptable Objects/WavesSO")]
public class WavesSO : ScriptableObject
{
    [SerializeField] private List<WaveData> waves = new();

    public WaveData GetData(int index) => waves[index];
    public Obstacle GetObstacle(int idWave, int idObstacle) => waves[idWave].obstacles[idObstacle];

    public int GetObstaclesCount(WaveData waveData) => waveData.obstacles.Count;
    public int WavesCount => waves.Count;
}
