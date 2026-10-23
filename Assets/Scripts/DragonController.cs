using System;
using System.Collections;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class DragonController : MonoBehaviour
{
    #region Fields
    [Foldout("References"),SerializeField] private Transform _dragonMouth;
    [Foldout("References"),SerializeField] private LayerMask _layerToBreathOn;

    [Header("Art")]
    [Foldout("References"), SerializeField] private GameObject _breathVFX;
    [Foldout("References"), SerializeField] private Animator _dragonAnimator;

    [Header("Fire breathing")]
    [SerializeField] private float _breathingDelay = 5f;
    [SerializeField] private float _breathingDuration = 3f;
    [SerializeField] private float _breathingDistance = 200;
    [SerializeField] private float _breathingRadius = 0.5f;
    [SerializeField] private int _flames = 5;

    //SOUND
    [Header("Sound")]
    [SerializeField] private float _flyFrequency = 1f;
    AudioSource _audioSource;

    //
    private bool _canBreath = false;

    //
    GameObject _planet;

    // splines
    SplineAnimate splineAnimate;

    

    #endregion

    #region Main
    private void Awake()
    {
        InitMySelf();
    }
    private void Start()
    {
        InitWithOthers();

        // fly sfx
        StartCoroutine("LoopFlySFX"); // in test

        // launch dragon breathing loop at start
        StartCoroutine("StartBreathingBehaviour");
    }
    #endregion

    #region Init
    private void InitMySelf()
    {
        //position
        if (!_dragonMouth) _dragonMouth = GetComponent<Transform>();

        // spline
        splineAnimate = GetComponent<SplineAnimate>();

        //vfx
        _breathVFX.SetActive(false);

        // sfx
        _audioSource = GetComponent<AudioSource>();
    }

    private void InitWithOthers()
    {
        // planet
        _planet = GameObject.FindGameObjectWithTag("Planet");

        // health
        GetComponent<Health>().OnDie += OnDie;
        GetComponent<Health>().OnDamage += OnDamage;
    }
    #endregion


    private IEnumerator StartBreathingBehaviour()
    {
        while(true){
            // reset for another breath
            _canBreath = false;
            ResetBreathingFeedbacks();

            // delay before breathing 
            yield return new WaitForSeconds(_breathingDelay); 
            StartCoroutine("BreathFire");

            // wait until current breath has ended
            yield return new WaitUntil(()=>_canBreath);
        }
    }

    private IEnumerator BreathFire()
    {
        PlayBreathFeedbacks();

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

    private void PlayBreathFeedbacks()
    {
        // Sound
        AudioManager.Instance.PlaySFX(SoundType.DragonBreath, _audioSource); // breath
        // Anim
        _dragonAnimator.SetTrigger("BreathFire");
        // vfx
        _breathVFX.SetActive(true);
    }

    private void ResetBreathingFeedbacks()
    {
        // vfx
        _breathVFX.SetActive(false);
    }

    public void FollowSpline(SplineContainer spline, float lapDuration)
    {
        splineAnimate.Container = spline;
        splineAnimate.Duration = lapDuration;
        splineAnimate.Play();
    }

    // broadcast in health component
    private void OnDamage(int dmg)
    {
       
    }

    // broadcast in health component
    private void OnDie()
    {
        // sound
        AudioManager.Instance.PlaySFX(SoundType.DragonDie, _audioSource);  // die

        Destroy(gameObject,1);
    }

    #region Sound

    // IN TEST
    private IEnumerator LoopFlySFX() 
    {

        AudioClip clip = AudioManager.Instance.GetAudioClip(SoundType.DragonFly);

        while (true)
        {
            _audioSource.PlayOneShot(clip);

            yield return new WaitForSeconds(_flyFrequency);
        }
    }
    #endregion
}
