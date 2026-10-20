
using UnityEngine;
using UnityEngine.Splines;

public class Controls : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] Transform _horizontalTransform;
    [SerializeField] Transform _verticalTransform;

    [SerializeField] Transform _planet;
    [SerializeField] GameObject _player;
    [SerializeField] Transform _ancre;

    [Header("Values")]
    [SerializeField] float _horizontalSpeed = 50f;
    [SerializeField] float _verticalSpeed = 50f;
    #endregion

    #region Main

    private void Awake()
    {
        InitAxisPos();
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePos();
    }
    #endregion

    private void InitAxisPos()
    {
        _horizontalTransform.position = _planet.position;
        _verticalTransform.position = _planet.position;

        //SplineContainer container;

        if (_horizontalTransform.gameObject.TryGetComponent<SplineContainer>(out var container))
        {
            var firstKnot = container.Spline.ToArray()[0];
            _ancre.localPosition = firstKnot.Position;
        }
        //else
        //{
        //_ancre.position = _planet.position + _horizontalTransform.localScale / 2;
        //}
    }

    // TO DEBUG VERTICAL ROTATION WITH QUAT
    void CalculatePos()
    {

        // -- PLAYER POS--
        // Input result
        // invert to link rotation direction to accurate move direction 
        if (ArduinoInputReceiver.Instance.UseKeyboard)
        {
            float y = _horizontalTransform.eulerAngles.y +  PlayerController.MoveVector.x * _horizontalSpeed * -1 * Time.deltaTime;
            float z = _verticalTransform.eulerAngles.z + PlayerController.MoveVector.y * _verticalSpeed * -1 * Time.deltaTime;
            _verticalTransform.rotation = Quaternion.Euler(_verticalTransform.rotation.x, y, z);
            _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, y, z);
        }
        else
        {
            float y = _horizontalTransform.eulerAngles.y +  (int)ArduinoInputReceiver.Instance.Encoder1Value * _horizontalSpeed * -1 * Time.deltaTime;
            float z = _verticalTransform.eulerAngles.z + (int)ArduinoInputReceiver.Instance.Encoder2Value * _verticalSpeed * -1 * Time.deltaTime;
            _verticalTransform.rotation = Quaternion.Euler(_verticalTransform.rotation.x, y, z);
            _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, y, z);
        }
        //float y = _horizontalTransform.rotation.y + PlayerController.MoveVector.x * -1 * _horizontalSpeed * Time.deltaTime;

        //float z = _verticalTransform.rotation.z + PlayerController.MoveVector.y * -1 *_verticalSpeed  * Time.deltaTime;

        



        //_verticalTransform.Rotate(new Vector3(_verticalTransform.rotation.x, y, z), Space.World);


        _player.transform.position = _ancre.position;

        // look at planet
        Quaternion lookRot = GetLookRotation();
        //Vector3 test = new Vector3(lookRot.x, lookRot.y, 0);
        //_player.transform.rotation = Quaternion.Euler(test);
        _player.transform.rotation = lookRot;
    }

    private Quaternion GetLookRotation()
    {
        Vector3 dir = _planet.position - _player.transform.position ;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        return lookRot;
    }

}
