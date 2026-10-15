using NaughtyAttributes;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FireManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshFilter _planetMesh;
    [SerializeField] GameObject _firePrefab;
    [SerializeField] Transform _fireParent;
    Transform _planet;
    Vector3 _position;

    Vector3[] _vertices;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _planet = _planetMesh.transform;
        _vertices = _planetMesh.mesh.vertices;
    }

    [Button]
    [System.Obsolete]
    void SpawnFireRandomly()
    {
        int r = Random.Range(0, _vertices.Length-1);
        _position = _vertices[r];

        // if planet is scaled
        _position.x *= _planet.localScale.x;
        _position.y *= _planet.localScale.y;
        _position.z *= _planet.localScale.z;

        _position += _planet.position;

        // rotation

        GameObject fire = Instantiate(_firePrefab, _position, GetLookRotation(), _fireParent);// rotation à faire plus tard
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_position, 0.1f);
    }

    private Quaternion GetLookRotation()
    {

        Vector3 dir = (_position - _planet.transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        return lookRot;
    }
}
