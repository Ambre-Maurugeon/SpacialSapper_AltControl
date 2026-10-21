
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

    [Header("Speed")]

    [SerializeField] float _maxSpeed = 50f; // for both axes
    [SerializeField] float _acceleration = 10f;
    [SerializeField] float _deceleration = 10f;

    float _horizontalSpeed = 0f;
    float _verticalSpeed = 0f;

    float _dirX;
    float _dirY;

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
        UpdateDir();
        UpdateSpeed();

        CalculatePos();
    }
    #endregion

    #region Init
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
    #endregion

    // TO DEBUG VERTICAL ROTATION WITH QUAT
    void CalculatePos()
    {

        // -- PLAYER POS--
        // Input result
        // invert to link rotation direction to accurate move direction 
        if (ArduinoInputReceiver.Instance.UseKeyboard)
        {
            float y = _horizontalTransform.eulerAngles.y +  _dirX * _horizontalSpeed * -1 * Time.deltaTime;
            float z = _verticalTransform.eulerAngles.z + _dirY * _verticalSpeed * -1 * Time.deltaTime;
            _verticalTransform.rotation = Quaternion.Euler(_verticalTransform.eulerAngles.x, y, z);
            _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.eulerAngles.x, y, z);
        }
        else
        {
            float y = _horizontalTransform.eulerAngles.y +  (int)ArduinoInputReceiver.Instance.Encoder1Value * _horizontalSpeed * -1 * Time.deltaTime;
            float z = _verticalTransform.eulerAngles.z + (int)ArduinoInputReceiver.Instance.Encoder2Value * _verticalSpeed * -1 * Time.deltaTime;
            _verticalTransform.rotation = Quaternion.Euler(_verticalTransform.eulerAngles.x, y, z);
            _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.eulerAngles.x, y, z);
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

    private void UpdateDir()
    {
        if(Mathf.Abs(PlayerController.MoveVector.x) > 0.01f) // if XInput != 0 => update dirX
            _dirX = PlayerController.MoveVector.x;

        if (Mathf.Abs(PlayerController.MoveVector.y) > 0.01f) // if YInput != 0 => update dirY
            _dirY = PlayerController.MoveVector.y;
    }

    #region Speed

    private bool IsHorizontalMoving => Mathf.Abs(PlayerController.MoveVector.x) >= 0.1f; // x tolerance
    private bool IsVerticalMoving => Mathf.Abs(PlayerController.MoveVector.y) >= 0.1f; // y tolerance

    private void UpdateSpeed()
    {
        // Horizontal speed
        if (IsHorizontalMoving)
            Accelerate(ref _horizontalSpeed);
        else
            Decelerate(ref _horizontalSpeed);

        // Vertical Speed
        if (IsVerticalMoving)
            Accelerate(ref _verticalSpeed);
        else
            Decelerate(ref _verticalSpeed);
    }

    private void Accelerate(ref float speed)
    {
        speed += _acceleration * Time.deltaTime;
        if (speed > _maxSpeed)
        {
            speed = _maxSpeed;
        }
    }

    private void Decelerate(ref float speed)
    {
        speed -= _deceleration * Time.deltaTime;
        if (speed < 0f)
        {
            speed = 0f;
        }
    }

    #endregion

}
