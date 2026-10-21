
using UnityEngine;
using UnityEngine.Splines;

public class Controls : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] Transform _horizontalTransform;
    [SerializeField] Transform _verticalTransform;

    [SerializeField] GameObject _player;
    [SerializeField] Transform _ancre;
    [SerializeField] Transform _playerTest;

    [SerializeField] Transform _ancreTest;

    [Header("Values")]
    [SerializeField] float _horizontalSpeed = 50f;
    [SerializeField] float _verticalSpeed = 50f;

    Transform _planet;
    #endregion

    #region Main

    private void Start()
    {
        InitWithOthers();
        InitAxisPos();
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePos();
    }
    #endregion

    private void InitWithOthers()
    {
        _planet = GameObject.FindGameObjectWithTag("Planet").transform;
    }
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

            ////
            //float y1 = PlayerController.MoveVector.x * _horizontalSpeed * Time.deltaTime;
            //float z1 =  PlayerController.MoveVector.y * _verticalSpeed * Time.deltaTime;
            ////_playerTest.localEulerAngles = r(_playerTest.rotation.x, y1, z1);
            _playerTest.Rotate(Vector3.right, PlayerController.MoveVector.y * 2 , Space.Self);
            _playerTest.Rotate(Vector3.up, PlayerController.MoveVector.x * 2 * -1, Space.Self);

            //float myAngle = Mathf.Atan2(y1, z1) * Mathf.Rad2Deg;
            //float bodyRotation = myAngle + _playerTest.transform.eulerAngles.y;

            ////apply the change to the gameObject
            ////_playerTest.localEulerAngles = new Vector3(_playerTest.localEulerAngles.x, y1, z1);
            //_playerTest.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, bodyRotation, z1);
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

        // player looks planet
        Quaternion lookRot = GeometryUtils.GetLookRotation(_player.transform.position, _planet.position);
        //Vector3 test = new Vector3(lookRot.x, lookRot.y, 0);
        //_player.transform.rotation = Quaternion.Euler(test);
        _player.transform.rotation = lookRot;
    }

}
