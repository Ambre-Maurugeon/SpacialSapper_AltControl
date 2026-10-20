
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterGun : MonoBehaviour
{
    [Foldout("References"),SerializeField] RectTransform _cursor;
    [Foldout("References"), SerializeField] Transform _barrel;
    [Foldout("References"), SerializeField] LineRenderer _ballisticLine;

    [Foldout("References"), SerializeField] LayerMask _waterDamageable; // change to enum in weakness health => use in dragon controller

    [Header("Cursor Values")]
    [SerializeField] float _clampPosX = 300 ;
    [SerializeField] float _clampPosY = 300 ;
    [Space(5)]
    [SerializeField] float _cursorHorizontalSpeed;
    [SerializeField] float _cursorVerticalSpeed;
    [Space(5)]
    [SerializeField] float _deadZoneAngleVertical;
    [SerializeField] float _deadZoneAngleHorizontal;

    [Header("Gun Values")]
    [SerializeField] float _firingDelay = 3;
    [SerializeField] float _firingDuration = 10;
    [SerializeField] float _waterRadius = 0.1f;
    [SerializeField] float _firingDistance = 2f;

    // For water vfx
    private float _maxDistance = 20; // 20 for now
    private float _currentDistance = 0;
    private float _placement = 0;// public for vfx

    [Header("Gun Dmg")]
    [SerializeField] int _waterDamage = 10;
    [SerializeField] float _damageFrequency = 2f;

    private bool _isShooting = false;

    RaycastHit hit; // DEBUG

    #region Main
    private void Awake()
    {
        InitMyself();
    }

    private void Start()
    {
        InitWithOthers();

        // move in ui manager
        HideMouseCursor();

    }

    private void Update()
    {
        CalculateCursorPos();
    }
    #endregion

    #region Init

    private void InitMyself()
    {
        _maxDistance = _firingDistance; //Init Water Vfx ( _maxDistance useless but for test)
    }
    private void InitWithOthers()
    {
        // link inputs
        PlayerController.OnReload += OnReload;
    }

    #endregion

    // move to ui manager script ?
    void HideMouseCursor()
    {
        Cursor.visible = false;
    }

    void CalculateCursorPos()
    {
        if (ArduinoInputReceiver.Instance.UseKeyboard){
            Vector3 mousePos = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor.parent as RectTransform, mousePos, null, out Vector2 localPos);
            localPos.x = Mathf.Clamp(localPos.x,-_clampPosX, _clampPosX);
            localPos.y = Mathf.Clamp(localPos.y,-_clampPosY, _clampPosY);

            _cursor.localPosition = localPos;
        }
        else
        {
            Vector3 cursorPos = _cursor.position;
            float yaw = ArduinoInputReceiver.Instance._yaw;
            float pitch = ArduinoInputReceiver.Instance._pitch;
            yaw = CheckDeadZones(yaw, pitch)[0];
            pitch = CheckDeadZones(yaw, pitch)[1];
            cursorPos.x -= yaw * _cursorHorizontalSpeed * Time.deltaTime;
            cursorPos.y -= pitch * _cursorVerticalSpeed * Time.deltaTime;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor.parent as RectTransform, cursorPos, null, out Vector2 localPos);
            localPos.x = Mathf.Clamp(localPos.x,-_clampPosX, _clampPosX);
            localPos.y = Mathf.Clamp(localPos.y,-_clampPosY, _clampPosY);

            _cursor.localPosition = localPos;
            
        }
        // clamp
        
    }

    void OnReload()
    {
        Debug.Log("delay, reload in " + _firingDelay + " seconds.");
        Invoke("StartShooting",_firingDelay);
    }

    void StartShooting()
    {
        Debug.Log("Start shooting");

        _isShooting = true;

        StartCoroutine("Shoot");
    }

    IEnumerator Shoot()
    {
        Debug.Log("je shoot");

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
            UpdateBallisticTrajectory(out hit);

            // it's dmg time !
            if (frequencyTime >= _damageFrequency) 
            {
                if (hit.collider != null)
                {
                    // if object is waterdmgeable && has health component => make dmg 
                    if ((_waterDamageable & (1 << hit.collider.gameObject.layer)) != 0 // check if layer of the hit "is found in" layerMask _waterDamageable
                        && hit.collider.gameObject.TryGetComponent<Health>(out var h))
                        h.TakeDamage(_waterDamage);
                }
                //else
                //{
                //    Debug.Log("no hit");
                //}

                // Reset timer 
                frequencyTime = 0;
            }
            yield return new WaitForEndOfFrame();

        }

        // -- STOP SHOOTING --
        ShowTrajectory(false);
        Debug.Log("Shooting ended");
    }

    private void UpdateBallisticTrajectory(out RaycastHit hit)
    {
        // ray from barrel to X distance
        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance); // default end pos

        Vector3 dir = (destination - _barrel.position).normalized;

        // raycast
        if (Physics.SphereCast(_barrel.position, _waterRadius, dir, out hit, _firingDistance)) // layerMask  to debug
        {
            // update end pos at contact
            destination = hit.point;
        }

        // line from start to end
        _ballisticLine.SetPosition(0, _barrel.position);
        _ballisticLine.SetPosition(_ballisticLine.positionCount - 1, destination);

        // Update Water Vfx Placement 
        //UpdateWaterVfxPlacement();
        _currentDistance = (_barrel.position - destination).magnitude;
        _placement = Mathf.Clamp(_currentDistance / _maxDistance, 0,1); // current distance between start and end pos, percent

        // Visuals
        ShowTrajectory(true);
    }

    private void ShowTrajectory(bool show)
    {
        _ballisticLine.enabled = show;
    }

    
    //private void UpdateWaterVfxPlacement()
    //{

        
    //}

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
        Gizmos.color = Color.green;

        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance);
        Gizmos.DrawSphere(destination, _waterRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, _waterRadius);

    }
    #endregion

}
