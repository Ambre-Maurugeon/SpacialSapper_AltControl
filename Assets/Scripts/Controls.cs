
using System.Collections;
using System.Drawing.Text;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;
using static Unity.VisualScripting.Member;

public class Controls : MonoBehaviour
{
    #region Fields
    [Header("Player")]
    [Foldout("References"), SerializeField] Transform _horizontalTransform; // to remove when arduino rotation is updated with new fixed rotation
    [Foldout("References"), SerializeField] Transform _verticalTransform; // to remove when arduino rotation is updated with new fixed rotation

    [Foldout("References"), SerializeField] GameObject _player;
    [Foldout("References"), SerializeField] Transform _ancre; // to remove when arduino rotation is updated with new fixed rotation
    [Foldout("References"), SerializeField] Transform _playerParent;

    [Header("Sounds")]
    [Foldout("References"),SerializeField] AudioSource _audioSourceVertical;
    [Foldout("References"), SerializeField] AudioSource _audioSourceHorizontal;


    [Header("Speed")]

    [SerializeField] float _maxSpeed = 2; // for both axes
    [SerializeField] float _acceleration = 1f;
    [SerializeField] float _deceleration = 1f;

    float _horizontalSpeed = 0f;
    float _verticalSpeed = 0f;

    [Header("Sound")]
    [SerializeField] float _horizontalSoundFrequency = 0.75f;
    [SerializeField] float _verticalSoundFrequency = 0.75f;

    bool _isPlayingHorizontalSound = false;
    bool _isPlayingVerticalSound = false;

    [Header("Art")]
    [SerializeField] Transform _verticalRoad;

    // planet
    Transform _planet;

    // dir
    float _dirX;
    float _dirY;


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

        // sound
        UpdateSound();

        // visual effet, rotate road
        if (Mathf.Abs(_verticalSpeed)>0)
        {
            _verticalRoad.Rotate(10 * Time.deltaTime , 0f, 0f, Space.Self);
        }
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

        // -- PLAYER POS--
        // Input result
        // invert to link rotation direction to accurate move direction 
        if (ArduinoInputReceiver.Instance._useKeyboard)
        {
            _playerParent.Rotate(Vector3.right, _dirY * _verticalSpeed , Space.Self);
            _playerParent.Rotate(Vector3.up, _dirX * _horizontalSpeed* -1, Space.Self); // invert to link rotation direction to accurate move direction 

        }
        else
        {
            float y = _horizontalTransform.eulerAngles.y +  (int)ArduinoInputReceiver.Instance._encoder1Value * _horizontalSpeed * -1 * Time.deltaTime;
            float z = _verticalTransform.eulerAngles.z + (int)ArduinoInputReceiver.Instance._encoder2Value * _verticalSpeed * -1 * Time.deltaTime;
            _verticalTransform.rotation = Quaternion.Euler(_verticalTransform.rotation.x, y, z);
            _horizontalTransform.rotation = Quaternion.Euler(_horizontalTransform.rotation.x, y, z);
        }


        //_player.transform.position = _ancre.position;
        //_player.transform.rotation = _ancre.rotation ;
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

    #region Sound
    private void UpdateSound()
    {
        // Horizontal sound
        if (!_isPlayingHorizontalSound)
        {
            if (_horizontalSpeed > 0)
                StartCoroutine("LoopHorizontalSound");
            else
                _isPlayingHorizontalSound = false;
                
        }

        // Vertical sound
        if (!_isPlayingVerticalSound)
        {
            if (_verticalSpeed > 0)
                StartCoroutine("LoopVerticalSound");
            else
                _isPlayingVerticalSound = false;

        }
    }

    private IEnumerator LoopHorizontalSound()
    {
        _isPlayingHorizontalSound = true;

        // if is horizontal moving => play sound each x sec
        float time = 0;
        while (_horizontalSpeed>0)
        {
            time += Time.deltaTime; // * _horizontalSpeed; // sound frequency depends on player speed

            if (time >= _horizontalSoundFrequency)// sound frequency
            {
                _audioSourceHorizontal.pitch = Random.Range(0.0f, 1.0f);
                _audioSourceHorizontal.Play();
                time = 0;
            }
            yield return new WaitForEndOfFrame();
        }
        _isPlayingHorizontalSound = false;
    }

    private IEnumerator LoopVerticalSound()
    {
        _isPlayingVerticalSound = true;

        // if is horizontal moving => play sound each x sec
        float time = 0;
        while (_verticalSpeed > 0)
        {
            time += Time.deltaTime; // * _verticalSpeed;

            if (time >= _verticalSoundFrequency)// sound frequency
            {
                _audioSourceVertical.pitch = Random.Range(0.0f, 1.0f);
                _audioSourceVertical.Play();
                time = 0;
            }
            yield return new WaitForEndOfFrame();
        }
        _isPlayingVerticalSound = false;
    }
    #endregion


}
