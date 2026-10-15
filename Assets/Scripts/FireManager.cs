using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class FireManager : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [SerializeField] MeshFilter _planetMesh;
    [SerializeField] GameObject _firePrefab;
    [SerializeField] Transform _fireParent;

    Transform _planet;
    Vector3[] _vertices;

    #endregion

    public static FireManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Init();
    }

    private void Init()
    {
        _planet = _planetMesh.transform;
        _vertices = _planetMesh.mesh.vertices;
    }

    [Button]
    void SpawnFireRandomly()
    {
        // get rd vertice pos
        int r = Random.Range(0, _vertices.Length-1);
        Vector3 position = _vertices[r];

        // fix pos if planet is scaled
        position.x *= _planet.localScale.x;
        position.y *= _planet.localScale.y;
        position.z *= _planet.localScale.z;

        position += _planet.position;

        // spawn fire at pos
        SpawnFire(position);
    }

    public void SpawnFire(Vector3 position)
    {
        Quaternion lookRot = GetLookRotation(position);
        Instantiate(_firePrefab, position, lookRot, _fireParent);
    }

    private Quaternion GetLookRotation(Vector3 position)
    {
        Vector3 dir = (position - _planet.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        return lookRot;
    }
}
