using System;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class DragonController : MonoBehaviour
{
    #region Fields
    [Foldout("References")] [SerializeField] private Transform _dragonMouth;
    [Foldout("References")] [SerializeField] private LayerMask _layerToBreathOn;

    [Header("Dragon Path")]
    [SerializeField] private float _lapDuration = 5f;

    [Header("Fire breathing")]
    [SerializeField] private float _breathingDelay = 5f;
    [SerializeField] private float _breathingDuration = 3f;
    [SerializeField] private float _breathingDistance = 200;
    [SerializeField] private float _breathingRadius = 0.5f;
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

        // launch dragon breathing loop at start
        StartCoroutine("StartBreathingBehaviour");
    }

    private void Init()
    {
        if (!_dragonMouth) _dragonMouth = GetComponent<Transform>();

        GetComponent<SplineAnimate>().Duration = _lapDuration;
    }

    private IEnumerator StartBreathingBehaviour()
    {
        while(true){
            _canBreath = false;

            // delay before breathing 
            yield return new WaitForSeconds(_breathingDelay); 
            StartCoroutine("BreathFire");

            // wait until current breath has ended
            yield return new WaitUntil(()=>_canBreath);
        }
    }

    private IEnumerator BreathFire()
    {
        float flameFrequency = _breathingDuration/_flames; // get flame frequency for each breath (thx to breathing duration & flames nb)

        float time = 0;
        float frequencyTime = flameFrequency;

        // spawn fires every x time during the duration of the spit
        while (time <= _breathingDuration)
        {
            time += Time.deltaTime;
            frequencyTime += Time.deltaTime;

            // it's flame time !
            if (frequencyTime >= flameFrequency) 
            {
                // cast a ray from dragon to planet to get a hit point
                Vector3 dir = (_planet.transform.position - _dragonMouth.position).normalized;
                if (Physics.Raycast(_dragonMouth.position, dir, out RaycastHit hit, _breathingDistance, _layerToBreathOn)) // breath fire only on planet for now
                {
                    // spawn fire on the planet, at a random point near planet hit point
                    FireManager.Instance?.SpawnFireNear(hit.point, _breathingRadius);
                }
                frequencyTime = 0;
            }

            yield return new WaitForEndOfFrame();
        }

        _canBreath = true;
    }

}
