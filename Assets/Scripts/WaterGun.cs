
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WaterGun : MonoBehaviour
{
    [Foldout("References"),SerializeField] RectTransform _cursor;
    [Header("Barrel")]
    [Foldout("References"), SerializeField] Transform _barrelAnchor;
    [Foldout("References"), SerializeField] Transform _barrel;

    [Header("Water jet")]
    [Foldout("References"), SerializeField] LineRenderer _ballisticLine;
    [Foldout("References"), SerializeField] Renderer _waterRenderer;
    // Transform _ImpactAnchor;
    List<Material> _waterJetMaterials = new();

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
    private float _currentDistance = 0;
    private float _waterAdvance = 0;// public for vfx

    [Header("Gun Dmg")]
    [SerializeField] int _waterDamage = 10;
    [SerializeField] float _damageFrequency = 2f;
    //[SerializeField] float _damageDuration = 1f;

    private bool _isShooting = false;

    RaycastHit _hit; // DEBUG

    #region Main
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

    private void InitWithOthers()
    {
        // link inputs
        PlayerController.OnReload += OnReload;

        // water jet
        _waterRenderer.GetMaterials(_waterJetMaterials); // init material list
        ResetWaterJet(); // start without water jet

    }

    #endregion

    // move to ui manager script ?
    void HideMouseCursor()  
    {
        UnityEngine.Cursor.visible = false;
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

        // -- TO EDIT/ CLEAN jsuis fatiguee --
        // barrel roation, move barrel look at rot
        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance);
        _barrel.transform.rotation = GeometryUtils.GetLookRotation(_barrel.transform.position, destination);
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
        ShowTrajectory(false); // pink line, debug for now
        ResetWaterJet();
        Debug.Log("Shooting ended");
    }


    private void UpdateBallisticTrajectory(out RaycastHit[] hits)
    {
        // ray from barrel to X distance
        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance); // default end pos

        Vector3 dir = (destination - _barrelAnchor.position).normalized;

        // Get all touched objects
        hits = Physics.SphereCastAll(_barrelAnchor.position, _waterRadius, dir, _firingDistance); // layerMask  to edit

        // -- TO EDIT/ CLEAN jsuis fatiguee --
        // raycast when firing to get end pos of water jet, by impact point
        if (Physics.Raycast(_barrelAnchor.position, dir, out _hit, _firingDistance)) // layerMask  to debug
        {
            // update end pos at contact
            destination = _hit.point;
        }


        // line from start to end
        _ballisticLine.SetPosition(0, _barrelAnchor.position);
        _ballisticLine.SetPosition(_ballisticLine.positionCount - 1, destination);

        // Update Water Vfx Placement 
        UpdateWaterJetAdvance(destination);

        // move barrel look at rot
        _barrel.transform.rotation = GeometryUtils.GetLookRotation(_barrel.transform.position,destination); // to call in calculate mouse pos => which destination

        // TO EDIT scale y of water jet = firingDistance => does't work ?

        // Visuals
        ShowTrajectory(true); // pink line, debug for now
    }

    private void ShowTrajectory(bool show)
    {
        _ballisticLine.enabled = show;
    }

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

    private void ResetWaterJet() // doesn't work ? why
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
        Gizmos.DrawSphere(_hit.point, _waterRadius);

    }
    #endregion

}
