using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;

public class Controls : MonoBehaviour
{
    #region Fields
    [SerializeField] Transform _horizontalTransform;
    [SerializeField] Transform _verticalTransform;

    [SerializeField] Transform _planet;
    [SerializeField] Transform _ancre;

    [SerializeField] GameObject _player;

    [Header("Values")]
    [SerializeField] float _horizontalSpeed = 50f;
    [SerializeField] float _verticalSpeed = 50f;
    #endregion

    #region Main
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePos();
    }
    #endregion

    void CalculatePos()
    {
        // look at planet
        Quaternion lookRot = GetLookRotation();
        _player.transform.rotation = lookRot;

        // calculate input move result
        float y = _horizontalTransform.eulerAngles.y + PlayerController.MoveVector.x*_horizontalSpeed*Time.deltaTime; 
        float z = _verticalTransform.eulerAngles.z + PlayerController.MoveVector.y * _verticalSpeed * Time.deltaTime;

        _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, y, z);
        _verticalTransform.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, y, z);


        _player.transform.position = _ancre.position;
    }

    private Quaternion GetLookRotation()
    {
        Vector3 dir = _planet.position - _player.transform.position ;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        return lookRot;
    }
}
