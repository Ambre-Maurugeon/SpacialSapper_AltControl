using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [Foldout("References"),SerializeField] private WavesSO _wavesDbMgr;

    [Header("Waves")]
    [SerializeField] private float _intervalTime = 15;

    private bool _wavesAchieved = false;

   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
        StartCoroutine("StartWaves");
    }

    private IEnumerator StartWaves()
    {
        float time = 0;

        int currentWaveIndex = 0;
        WaveData currentWave = _wavesDbMgr.GetData(currentWaveIndex);

        int currentObstacleIndex = 0; // for now increment obstacle index
        Obstacle currentObstacle = _wavesDbMgr.GetObstacle(currentWaveIndex, currentObstacleIndex);

        bool wavesInterval = false;

        // wait until end of game => use a bool
        while (!_wavesAchieved)
        {
            time += Time.deltaTime;

            if (wavesInterval)
            {
                // it's new wave time !
                if (time >= _intervalTime)
                {
                    time = 0;
                    wavesInterval = false;
                }
            }

            // it's obstacle time !
            else if (time >= currentObstacle.time) // else if for now but edit method archi
            {
                // spawn obstacle
                // make a method for this with dico ?
                switch (currentObstacle.type)
                {
                    case OBSTACLE.DRAGON:
                        DragonManager.Instance.SpawnDragonOnRdSpline();
                        break;
                    case OBSTACLE.METEORITE:
                        MeteoritesManager.Instance.GenerateMeteorite(currentObstacle.id);
                        break;
                    default:
                        break;
                }

                // next obstacle if there is 
                if(currentObstacleIndex < _wavesDbMgr.GetObstaclesCount(currentWave) - 1)
                {
                    currentObstacleIndex ++; // for now increment obstacle index
                    currentObstacle = _wavesDbMgr.GetObstacle(currentWaveIndex, currentObstacleIndex);
                }
                // next wave => reset index + increase wave index
                else
                {
                    // if it remains waves 
                    if(currentWaveIndex < _wavesDbMgr.WavesCount - 1)
                    {
                        // update current wave
                        currentWaveIndex++;
                        currentWave = _wavesDbMgr.GetData(currentWaveIndex);

                        // update current obstacle
                        currentObstacleIndex = 0; // reset obstacles index
                        currentObstacle = _wavesDbMgr.GetObstacle(currentWaveIndex, currentObstacleIndex);

                        // reset time & wait wave interval time 
                        wavesInterval = true;
                        time = 0;
                    }
                    // all waves are achieved => FINISH 
                    else
                    {
                        _wavesAchieved = true;
                        Debug.Log("Waves achieved");
                        yield return null;
                    }

                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
