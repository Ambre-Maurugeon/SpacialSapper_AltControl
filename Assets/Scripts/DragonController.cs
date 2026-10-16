using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class DragonController : MonoBehaviour
{
    #region Fields
    [Foldout("References")]
    [SerializeField] private Transform _dragonMouth;

    [Header("Values")]
    [SerializeField] private float _lapDuration = 5f;
    [SerializeField] private float _breathingDelay = 5f;
    [SerializeField] private float _breathingDuration = 3f;
    [SerializeField] private float _breathingDistance = 200;
    [SerializeField] private float _breathingRadius = 0.5f;
    [SerializeField] private int _flames = 5;

    private bool _canBreath = false;
    GameObject _planet;

    // test
    //RaycastHit[] hits;


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
                if (Physics.Raycast(_dragonMouth.position, dir, out RaycastHit hit, _breathingDistance)) // to debug : use layerMask
                {
                    FireManager.Instance?.SpawnFire(hit.point);
                }
                frequencyTime = 0;
            }

            yield return new WaitForEndOfFrame();
        }

        _canBreath = true;
    }

    [Button]
    private void TestSphereCast()
    {
        Vector3 dir = (_planet.transform.position - _dragonMouth.position).normalized;
        RaycastHit[] hits = Physics.SphereCastAll(_dragonMouth.position, _breathingRadius, dir, _breathingDistance);

        if (hits.Length != 0) // to debug : use layerMask
        {
            Debug.Log(hits.Length + " hit.s sphere test detected");
            int r = Random.Range(0, hits.Length-1);

            FireManager.Instance?.SpawnFire(hits[r].point);
        }
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawSphere(_dragonMouth.position, _breathingRadius);

        //Gizmos.color = Color.yellow;

        //if (UnityEngine.Application.isPlaying)
        //{
        //    foreach (RaycastHit hit in hits)
        //    {
        //        Gizmos.DrawSphere(hit.point, 0.5f);
        //    }
        //}

    }
    #endregion
}
