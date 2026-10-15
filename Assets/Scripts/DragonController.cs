using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class DragonController : MonoBehaviour
{
    #region Fields
    [Foldout("References")]
    [SerializeField] private Transform _dragonMouth;

    [Header("Values")]
    [SerializeField] private float _lapDuration = 5f;
    [SerializeField] private float _breathingDelay = 5f;
    [SerializeField] private float _breathingDuration = 3f;
    [SerializeField] private int _flames = 5;

    private bool _canBreath = false;
    GameObject _planet;
    #endregion
    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        _planet = GameObject.FindGameObjectWithTag("Planet");

        StartCoroutine("LaunchBreathingBehaviour");
    }

    private void Init()
    {
        if (!_dragonMouth) _dragonMouth = GetComponent<Transform>();

        GetComponent<SplineAnimate>().Duration = _lapDuration;
    }

    private IEnumerator LaunchBreathingBehaviour()
    {
        while(true){
            _canBreath = false;

            yield return new WaitForSeconds(_breathingDelay);
            StartCoroutine("BreathFire");

            yield return new WaitUntil(()=>_canBreath);
        }
    }

    private IEnumerator BreathFire()
    {
        float flameFrequency = _breathingDuration/_flames;

        float time = 0;
        float frequencyTime = flameFrequency;

        while (time <= _breathingDuration)
        {
            time += Time.deltaTime;
            frequencyTime += Time.deltaTime;

            if (frequencyTime >= flameFrequency) 
            {
                Vector3 dir = (_planet.transform.position - _dragonMouth.position).normalized;
                if (Physics.Raycast(_dragonMouth.position, dir, out RaycastHit hit, 200)) // to debug : use layerMask
                {
                    FireManager.Instance?.SpawnFire(hit.point);
                }
                frequencyTime = 0;
            }

            yield return new WaitForEndOfFrame();
        }

        _canBreath = true;
    }
}
