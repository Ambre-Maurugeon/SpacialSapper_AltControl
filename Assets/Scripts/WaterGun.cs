
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class WaterGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RectTransform _cursor;
    [SerializeField] Transform _barrel;

    [Header("Cursor Values")]
    [SerializeField] float _speedX = 30 ;
    [SerializeField] float _speedY = 30 ;

    [Space(5)]
    [SerializeField] float _clampPosX = 300 ;
    [SerializeField] float _clampPosY = 300 ;

    [Header("Gun Values")]
    [SerializeField] float _firingDelay = 3;
    [SerializeField] float _firingDuration = 10;
    [SerializeField] float _waterRadius = 0.1f;
    [SerializeField] float _firingDistance = 2f;

    [Header("Gun Dmg")]
    [SerializeField] int _waterDamage = 10;
    [SerializeField] float _damageFrequency = 2f;

    private bool _isShooting = false;

    RaycastHit hit; // DEBUG

    #region Main

    private void Start()
    {
        // link inputs
        PlayerController.OnReload += OnReload;

        //
        HideMouseCursor();
    }

    private void Update()
    {
        CalculateCursorPos();
    }
    #endregion

    void HideMouseCursor()
    {
        Cursor.visible = false;
    }

    void CalculateCursorPos()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        // clamp
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor.parent as RectTransform,mousePos, null, out Vector2 localPos);
        localPos.x = Mathf.Clamp(localPos.x,-_clampPosX, _clampPosX);
        localPos.y = Mathf.Clamp(localPos.y,-_clampPosY, _clampPosY);

        _cursor.localPosition = localPos;
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

        while (_isShooting)
        {
            time += Time.deltaTime;
            frequencyTime -= Time.deltaTime;

            if (time >= _firingDuration)
            {
                _isShooting=false;
                break;
            }

            if (frequencyTime <= 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
                Vector3 destination = ray.GetPoint(_firingDistance);
                Vector3 dir = (destination - _barrel.position).normalized;

                if (Physics.SphereCast(_barrel.position, _waterRadius, dir, out hit, _firingDistance))
                {
                    Debug.Log("hit");
                    if (hit.collider.gameObject.TryGetComponent<Health>(out var h))
                    {
                        Debug.Log("takedmg");
                        h.TakeDamage(_waterDamage);
                    }

                }
                else
                {
                    Debug.Log("no hit");
                }

                // Debug
                Debug.DrawLine(ray.origin, _barrel.position + _firingDistance * dir);

                // Reset timer 
                frequencyTime = _damageFrequency;
            }
            yield return new WaitForEndOfFrame();

        }

        Debug.Log("Shooting ended");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Ray ray = Camera.main.ScreenPointToRay(_cursor.position);
        Vector3 destination = ray.GetPoint(_firingDistance);
        Gizmos.DrawSphere(destination, _waterRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, _waterRadius);

    }

}
