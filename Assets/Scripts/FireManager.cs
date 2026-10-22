using System.Linq;
using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class FireManager : MonoBehaviour
{
    #region Fields

    [Header("Fire")]
    [Foldout("References")][SerializeField] GameObject _firePrefab;
    [Foldout("References")][SerializeField] Transform _fireParent;
    [Foldout("References")][SerializeField] LayerMask _fireDamageable;

    [Header("Spawn")]
    [SerializeField] float _collisionDetectionRadius = 0.5f;

    Transform _planet;
    Vector3[] _vertices;

    #endregion

    public static FireManager Instance { get; private set; }

    private void Awake()
    {
        // -- INSTANCE --
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitWithOthers();
    }

    private void InitWithOthers()
    {
        // register planet vertices
        _planet = GameObject.FindGameObjectWithTag("Planet").transform;
        _vertices = _planet.GetComponent<MeshFilter>()?.mesh.vertices;
    }

    [Button]
    void SpawnFireRandomly()
    {
        // get rd vertice pos
        int r = UnityEngine.Random.Range(0, _vertices.Length-1);
        Vector3 position = _vertices[r];

        // fix pos if planet is scaled
        position.x *= _planet.localScale.x;
        position.y *= _planet.localScale.y;
        position.z *= _planet.localScale.z;

        position += _planet.position;

        // spawn fire at pos
        SpawnFire(position);
    }

    private void SpawnFire(Vector3 position)
    {
        // spawn fire on planet at an accurate rotation
        Quaternion lookRot = GeometryUtils.GetLookRotation(_planet.position,position);

        Instantiate(_firePrefab, position, lookRot, _fireParent);
    }

    public void SpawnFireNear(Vector3 target, float radius)
    {
        // -- filter vertice array using LINQ requests --

        // hit point on planet is the default nearby pos
        Vector3 nearbyPos = target;

        // filter the vertices to keep only those that are in a sphere of radius x
        var posQuery = _vertices.Select(pos => _planet.TransformPoint(pos)) // convert vertice local transform to world transform (pos / rot / scale)
                                .Where(pos => (pos - target).sqrMagnitude <= radius * radius); // select if new world pos is in a X-radius sphere

        // update nearbyPos with a rd vertices in the sphere
        if (posQuery.Count() > 0)
        {
            int r = UnityEngine.Random.Range(0, posQuery.Count() - 1);
            nearbyPos = posQuery.ElementAt(r);
        }
        
        if(!CollisionDetected(nearbyPos)) // check collision with object, if collision => don't spawn for now
            SpawnFire(nearbyPos);
    }

    private bool CollisionDetected(Vector3 position)
    {
        // check collision with a sphere at the position of future fire
        if (Physics.CheckSphere(position, _collisionDetectionRadius, _fireDamageable))
            return true;

        return false;
           
    }
}
