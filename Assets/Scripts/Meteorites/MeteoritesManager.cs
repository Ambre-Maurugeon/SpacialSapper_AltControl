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
        DontDestroyOnLoad(this.gameObject);
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

        // at the stric opposite for now
        Vector3 dir = (_planet.position - _player.position).normalized; // dir from player to planet
        SpawnMeteorite(dir*_meteoriteDistance, data.prefab, out GameObject meteorite); // spawn X met at the opposite of player at x distance from planet

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

    #region Debug

    [Button]
    private void SpawnMeteoriteX()
    {
        GenerateMeteorite(_idMeteoriteX);
    }
    #endregion

}
