using System;
using NaughtyAttributes;
using UnityEngine;

public class MeteoritesManager : MonoBehaviour
{
    #region Fields
    [Foldout("References"),SerializeField] MeteoritesSO _meteoriteDbMgr;
    [Foldout("References"), SerializeField] Transform _meteoriteParent;

    [Header("Spawn")]
    [SerializeField] float _meteoriteDistance = 100;
    [SerializeField, Range(0f, 179f)] float _safeAngle = 40; // 180 deg is enough to compare two dir, can't be more

    [Header("Debug")]
    [SerializeField] string _idMeteoriteX = "M_A";

    Transform _player;
    Transform _planet;

    public static MeteoritesManager Instance { get; private set; }
    #endregion

    #region Main

    private void Awake()
    {
        // -- INSTANCE --
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        InitWithOthers();
    }
    #endregion

    #region Init
    private void InitWithOthers()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _planet = GameObject.FindGameObjectWithTag("Planet").transform;
    }
    #endregion


    public void GenerateMeteorite(string id)
    {
        MeteoriteData data = _meteoriteDbMgr.GetData(id);

        // at the stric opposite
        //Vector3 dir = (_planet.position - _player.position).normalized; // dir from player to planet

        // random with safe angle
        Vector3 playerDir = (_player.position - _planet.position).normalized; // dir from planet to player to compare with dot product
        Vector3 meteoriteDir = GetRandomSpawnDirection(playerDir); 

        SpawnMeteorite(meteoriteDir * _meteoriteDistance, data.prefab, out GameObject meteorite); // spawn X met at the opposite of player at x distance from planet

        meteorite.GetComponent<MeteoriteController>()?.OnSpawn(data);  // to edit rename method

    }

    private void SpawnMeteorite(Vector3 position, GameObject prefab, out GameObject meteorite)
    {
        // rotation look at planet
        // meteorite is facing the planet, ready to rush planet
        Quaternion lookRot = GeometryUtils.GetLookRotation(position, _planet.position);

        // create meteorite instance 
        meteorite = Instantiate(prefab, position, lookRot, _meteoriteParent); 
    }

    private Vector3 GetRandomSpawnDirection(Vector3 playerDir)
    {
        float minDot = Mathf.Cos(_safeAngle * Mathf.Deg2Rad);

        float dot;
        Vector3 spawnDir;

        do
        {
            spawnDir = UnityEngine.Random.onUnitSphere; // Returns a random point on the surface of a sphere with radius 1.0
            dot = Vector3.Dot(spawnDir, playerDir);
        }
        while (dot > minDot); // dot = 1  same dir ; dot = 0 perpendicular ; dot = -1 opposite

        return spawnDir;
    }

    #region Debug

    [Button]
    private void SpawnMeteoriteX()
    {
        GenerateMeteorite(_idMeteoriteX);
    }
    #endregion

}
