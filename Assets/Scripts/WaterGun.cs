

using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class WaterGun : MonoBehaviour
{
    #region Fields
    [Foldout("References"),SerializeField] RectTransform _cursor;
    [Header("Barrel")]
    [Foldout("References"), SerializeField] Transform _barrelAnchor;
    [Foldout("References"), SerializeField] Transform _barrel;

    [Header("Water jet")]
    [Foldout("References"), SerializeField] Renderer _waterRenderer;
    [Foldout("References"), SerializeField] AudioSource _audioWaterSource;

    private float _currentJetDistance = 0f;

    // Transform _ImpactAnchor;

    List<Material> _waterJetMaterials = new();
    private float _currentDistance = 0;
    private float _waterAdvance = 0;

    [Foldout("References"), SerializeField] LayerMask _waterDamageable; // change to enum in weakness health => use in dragon controller

    [Header("VFX")]
    [Foldout("Art References"),SerializeField] GameObject _waterImpactVFX;

    [Header("Water Reload")]
    [Foldout("Art References"), SerializeField] Transform _uiWaterLevel;
    [Foldout("Art References"), SerializeField] Transform _waterReloadUp;
    [Foldout("Art References"), SerializeField] Transform _waterReloadDown;

    [Header("Cursor Values")]
    [SerializeField] float _clampPosX = 300 ;
    [SerializeField] float _clampPosY = 300 ;
    [Space(5)]
    [SerializeAs("_cursorHorizontalSpeed")][SerializeField] float _arduinoCursorHorizontalSpeed;
    [SerializeAs("_cursorVerticalSpeed")][SerializeField] float _arduinoCursorVerticalSpeed;
    [Space(5)]
    [SerializeField] float _joystickCursorHorizontalSpeed = 3;
    [SerializeField] float _joystickCursorVerticalSpeed = 3;
    [Space(5)]
    [SerializeField] float _deadZoneAngleVertical;
    [SerializeField] float _deadZoneAngleHorizontal;

    Vector3 _currentCursorPosition;

    [Header("Gun Values")]
    [SerializeField] float _firingDelay = 3;
    [SerializeField] float _firingDuration = 10;
    [SerializeField] float _waterRadius = 0.1f;
    [SerializeField] float _firingDistance = 2f;

    [Header("Gun Dmg")]
    [SerializeField] int _waterDamage = 10;
    [SerializeField] float _damageFrequency = 2f;

    [Header("Water Jet")]
    [SerializeField] float _jetExtendDuration = 0.3f;
    [SerializeField] float _waterJetRotationSpeed = 360f;


    private bool _isShooting = false;
    private bool _isReloading = false;

    // Ballistic trajectory
    RaycastHit _hit; // to get the end position (destination => hit point) of the water s ballistic tractory

    private float _yaw;
    private float _pitch;

    // For Arduino reload Timer
    [Header("Arduino Reload")]
    [SerializeField] float _arduinoReloadTimer;
    private float _lastArduinoReload;

    // sound
    AudioSource _audioSource;
    #endregion

    #region Main
    private void Start()
    {
        InitWithOthers();
    }

    private void Update()
    {
        CalculateCursorPos();

        FeedbacksUpdate();
    }

   
    #endregion

    #region Init

    private void InitWithOthers()
    {
        // link inputs
        PlayerController.OnKeyboardReload += OnReload;
        PlayerController.OnArduinoReload += OnArduinoReload;

        // water jet
        _waterRenderer.GetMaterials(_waterJetMaterials); // init material list
        ResetWaterJet(); // start without water jet

        // water reload
        StartCoroutine(UpdateReloadWaterLevel(_waterReloadUp, _waterReloadDown, 1));

    }

    #endregion

   
    void CalculateCursorPos()
    {
        if (ArduinoInputReceiver.Instance._useKeyboard)
        {
            Vector3 cursorPos = Vector3.zero;

            // use mouse
            if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
            {
                // get mouse pos on screen
                cursorPos = Mouse.current.position.ReadValue();
                _currentCursorPosition = cursorPos;
            }
            // use right joystick of the gamepad
            else
            {
                // in.decrease current x / y cursor pos using look vector and x / y speed
                _currentCursorPosition.x += PlayerController.LookVector.x * _joystickCursorVerticalSpeed * Time.deltaTime;
                _currentCursorPosition.y += PlayerController.LookVector.y * _joystickCursorHorizontalSpeed * Time.deltaTime;

                _currentCursorPosition.x = Mathf.Clamp(_currentCursorPosition.x, 0, Screen.width);
                _currentCursorPosition.y = Mathf.Clamp(_currentCursorPosition.y, 0, Screen.height);

                cursorPos = _currentCursorPosition;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor.parent as RectTransform, cursorPos, null, out Vector2 localPos);
            localPos.x = Mathf.Clamp(localPos.x,-_clampPosX, _clampPosX);
            localPos.y = Mathf.Clamp(localPos.y,-_clampPosY, _clampPosY);

            _cursor.localPosition = localPos;
        }
        else
        {
            Vector3 cursorPos = _cursor.position;
            _yaw = ArduinoInputReceiver.Instance._yaw;
            _pitch = ArduinoInputReceiver.Instance._pitch;
            _yaw = CheckDeadZones(_yaw, _pitch)[0];
            _pitch = CheckDeadZones(_yaw, _pitch)[1];
            cursorPos.x += _yaw * _arduinoCursorHorizontalSpeed * Time.deltaTime;
            cursorPos.y += _pitch * _arduinoCursorVerticalSpeed * Time.deltaTime;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor.parent as RectTransform, cursorPos, null, out Vector2 localPos);
            localPos.x = Mathf.Clamp(localPos.x,-_clampPosX, _clampPosX);
            localPos.y = Mathf.Clamp(localPos.y,-_clampPosY, _clampPosY);

            _cursor.localPosition = localPos;
            
        }

        // barrel roation, move barrel look at rot
        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance);

        UpdateBarrelRotation(destination);
    }

    void OnReload()
    {
        if (GameManager.Instance.State == GAME_STATE.DEFEAT) return;

        if (_isShooting || _isReloading) return;

        // reload, then shoot
        StartCoroutine(UpdateReloadWaterLevel(_waterReloadDown, _waterReloadUp, _firingDelay));
        Invoke("StartShooting",_firingDelay);

        // sound
        AudioManager.Instance.PlaySFX(SoundType.ReloadWater, _audioSource); // just on reload
    }
    
    void OnArduinoReload()
    {
        if (GameManager.Instance.State == GAME_STATE.DEFEAT) return;

        if (_isShooting || _isReloading) return;

        if (Time.time < _lastArduinoReload + _arduinoReloadTimer) return;
        _lastArduinoReload = Time.time;

        // reload, then shoot
        StartCoroutine(UpdateReloadWaterLevel(_waterReloadDown, _waterReloadUp, _firingDelay));
        Invoke("StartShooting",_firingDelay);

        // sound
        AudioManager.Instance.PlaySFX(SoundType.ReloadWater, _audioSource); // just on reload
    }


    void StartShooting()
    {
        // sound
        AudioManager.Instance.PlaySFX(SoundType.WaterFullReload, _audioSource); // water full reload it's time to shoot

        _isShooting = true;
        _currentJetDistance = 0f;

        StartCoroutine("Shoot");

        // visuals
        StartCoroutine(UpdateReloadWaterLevel(_waterReloadUp, _waterReloadDown, _firingDuration));
    }

    IEnumerator Shoot()
    {
        // sound
        _audioWaterSource.Play();

        //
        float time = 0;
        float frequencyTime = _damageFrequency;

        // make dmg every x time during the duration of the shoot
        while (_isShooting)
        {
            time += Time.deltaTime;
            frequencyTime += Time.deltaTime;

            // shooting is over
            if (time >= _firingDuration) // to edit ? delete _isShooting field?
            {
                _isShooting=false;
                break;
            }

            // shoot Trajectory
            UpdateBallisticTrajectory(out RaycastHit[] hits);

            // it's dmg time !
            if (frequencyTime >= _damageFrequency) 
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null)
                    {
                        // if object is waterdmgeable && has health component => make dmg 
                        if ((_waterDamageable & (1 << hit.collider.gameObject.layer)) != 0 // check if layer of the hit is found among _waterDamageable layerMasks
                            && hit.collider.gameObject.TryGetComponent<Health>(out var h))
                            h.TakeDamage(_waterDamage);
                    }
                }
                
                // Reset timer
                frequencyTime = 0;
            }
            yield return new WaitForEndOfFrame();

        }

        // -- STOP SHOOTING --
        //ShowTrajectory(false); // pink line, debug for now
        ResetWaterJet();
        _hit = new RaycastHit();


        // feedbacks
        // sound
        _audioWaterSource.Stop();
        AudioManager.Instance.PlaySFX(SoundType.EmptyBarrel, _audioSource);

    }


    private void UpdateBallisticTrajectory(out RaycastHit[] hits)
    {
        // extend water jet
        float extendSpeed = _firingDistance / _jetExtendDuration;

        _currentJetDistance = Mathf.MoveTowards(_currentJetDistance,_firingDistance,extendSpeed * Time.deltaTime);

        // get point aimed by mouse
        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 aimPoint = ray.GetPoint(_firingDistance);

        // dir from barrel to aimPoint by mouse
        Vector3 direction = (aimPoint - _barrelAnchor.position).normalized;

        // increase distance progressively
        float distance = _currentJetDistance;

        // get all detection hits
        hits = Physics.SphereCastAll(_barrelAnchor.position,_waterRadius,direction,distance,_waterDamageable);

        // default max destination
        Vector3 destination = _barrelAnchor.position + direction * distance;

        // hit point detection
        if (Physics.Raycast(_barrelAnchor.position,direction,out _hit,distance))
        {
            destination = _hit.point;
        }

        // visuals
        UpdateWaterJetAdvance(destination);
    }

    private void UpdateBarrelRotation(Vector3 destination)
    {
        // get mouse direction in local space of barrel
        Vector3 localDirection = _barrelAnchor.InverseTransformDirection(destination - _barrelAnchor.position);

        if (localDirection.sqrMagnitude < 0.001f)
            return;

        // Yaw => horizontale rot
        float yaw = Mathf.Atan2(localDirection.x,localDirection.z) * Mathf.Rad2Deg;

        // Pitch => verticale rot 
        float pitch = -Mathf.Atan2(localDirection.y,new Vector2(localDirection.x, localDirection.z).magnitude) * Mathf.Rad2Deg;

        // no roll
        _barrel.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void FeedbacksUpdate()
    {
        // Impact VFX => can be edit
        _waterImpactVFX.SetActive(_hit.collider != null);
        if (_hit.collider != null)
            _waterImpactVFX.transform.position = _hit.point;

        // visuals
        if (_isShooting)
        {
            _waterRenderer.transform.Rotate(0f, _waterJetRotationSpeed * Time.deltaTime * -1, 0f, Space.Self);
        }
    }

    #region WaterJet

    private void ApplyAdvanceOnWaterJet()
    {
        // apply advance value in mats
        for (int i = 0; i < _waterJetMaterials.Count; i++)
        {
            _waterJetMaterials[i].SetFloat("_Advance", _waterAdvance);
        }

        //// deplace impact collider
        //GetImpactPosition(); // for impact vfx
    }

    //private void GetImpactPosition() // for impact vfx
    //{
    //    // deplace collider
    //    Vector3 dir = _barrel.forward;
    //    _ImpactAnchor.position = _barrelAnchor.position + dir * _currentDistance;
    //}

    private void ResetWaterJet() 
    {
        _currentDistance = 0;
        _waterAdvance = 0; // to edit => do it progressively 
        ApplyAdvanceOnWaterJet();
    }

    private void UpdateWaterJetAdvance(Vector3 target)
    {
        _currentDistance = (_barrelAnchor.position - target).magnitude;
        _waterAdvance = Mathf.Clamp(_currentDistance / _firingDistance, 0, 1); // current distance between start and end pos, percent

        ApplyAdvanceOnWaterJet();
    }
    #endregion

    #region WaterReload

    IEnumerator UpdateReloadWaterLevel(Transform from, Transform to, float duration)
    {
        _isReloading = true;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            _uiWaterLevel.localPosition = Vector3.Lerp(from.localPosition, to.localPosition, t);
            yield return new WaitForEndOfFrame();
        }

        _uiWaterLevel.localPosition = to.localPosition;

        _isReloading = false;
    }

    #endregion

    private float[] CheckDeadZones(float yaw, float pitch)
    {
        if ((_deadZoneAngleVertical / 2) > yaw
            && (-_deadZoneAngleVertical / 2) < yaw)
        {
            yaw = 0;
        }
        if ((_deadZoneAngleHorizontal / 2) > pitch
            && (- _deadZoneAngleHorizontal / 2) < pitch)
        {
            pitch = 0;
        }
        return new float[] { yaw, pitch };
    }

    #region Debug
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; 

        Gizmos.color = Color.green;

        if (_cursor != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
            Vector3 destination = ray.GetPoint(_firingDistance);
            Gizmos.DrawSphere(destination, _waterRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_hit.point, _waterRadius);

    }
    #endregion

}
