
using UnityEngine;
using UnityEngine.Splines;

public class Controls : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] Transform _horizontalTransform; // to remove when arduino rotation is updated with new fixed rotation
    [SerializeField] Transform _verticalTransform; // to remove when arduino rotation is updated with new fixed rotation

    [SerializeField] GameObject _player;
    [SerializeField] Transform _ancre; // to remove when arduino rotation is updated with new fixed rotation
    [SerializeField] Transform _playerParent;

    [Header("Speed")]

    [SerializeField] float _maxSpeed = 2; // for both axes
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float _deceleration = 1f;

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
        InitAxisPos(); // to remove when arduino rotation is updated with new fixed rotation
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
    private void InitAxisPos() // to remove when arduino rotation is updated with new fixed rotation
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

    
    void CalculatePos()
    {

            _playerParent.Rotate(Vector3.right, _dirY * _verticalSpeed , Space.Self);
            _playerParent.Rotate(Vector3.up, _dirX * _horizontalSpeed* -1, Space.Self); // invert to link rotation direction to accurate move direction 

    }

    private void UpdateDir()
    {
        if (ArduinoInputReceiver.Instance._useKeyboard)
        {
            if(Mathf.Abs(PlayerController.MoveVector.x) > 0.01f) // if XInput != 0 => update dirX
                _dirX = PlayerController.MoveVector.x;

            if (Mathf.Abs(PlayerController.MoveVector.y) > 0.01f) // if YInput != 0 => update dirY
                _dirY = PlayerController.MoveVector.y;
        }
        else
        {
            if(Mathf.Abs(PlayerController.ArduinoMoveVector.x) > 0.01f) // if XInput != 0 => update dirX
                _dirX = PlayerController.ArduinoMoveVector.x;

            if (Mathf.Abs(PlayerController.ArduinoMoveVector.y) > 0.01f) // if YInput != 0 => update dirY
                _dirY = PlayerController.ArduinoMoveVector.y;
        }
    }

    #region Speed

    private bool IsHorizontalMoving;
    private bool IsVerticalMoving;

    private void GetMovements()
    {
        if (ArduinoInputReceiver.Instance._useKeyboard)
        {
            IsHorizontalMoving = Mathf.Abs(PlayerController.MoveVector.x) >= 0.1f; // x tolerance
            IsVerticalMoving = Mathf.Abs(PlayerController.MoveVector.y) >= 0.1f; // y tolerance
        }
        else
        {
            IsHorizontalMoving = Mathf.Abs(PlayerController.ArduinoMoveVector.x) >= 0.1f;
            IsVerticalMoving = Mathf.Abs(PlayerController.ArduinoMoveVector.y) >= 0.1f;
        }
    }
    
    private void UpdateSpeed()
    {
        GetMovements();
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
        if (speed >= _maxSpeed)
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
