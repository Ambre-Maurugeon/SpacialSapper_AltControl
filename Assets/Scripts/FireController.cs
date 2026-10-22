
using System;
using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
    #region Fields
    enum STATE
    {
        NONE = 0,
        YELLOW = 1,
        RED = 2,
    }

    STATE _state = STATE.YELLOW; // useless for now, but can be used later

    [Header("Damage")]
    [SerializeField] private float _harmlessTime = 10;
    [SerializeField] private int _redDmg = 10;
    [SerializeField] private float _dmgFrequency = 3;
    [SerializeField] private LayerMask _fireDamageable;

    [SerializeField] private float _dmgRadius = 0.075f;

    [Header("Art")]
    [SerializeField] private Material _yellowMat;
    [SerializeField] private Material _redMat;
    MeshRenderer myMesh; // only change mat for now when switching when changing the fire type

    [Header("Art")]
    [SerializeField] private ParticleSystem _deathPS;
    [SerializeField] private ParticleSystem _damagePS;
    [SerializeField] private GameObject _fireVFX;

    #endregion

    #region Main
    private void Awake()
    {
        InitMyself();
    }

    private void Start()
    {
        InitWithOthers();
        StartCoroutine("StartFireBehaviour");
    }
    #endregion

    #region Init
    private void InitMyself()
    {
        myMesh = GetComponent<MeshRenderer>();
    }

    private void InitWithOthers()
    {
        GetComponent<Health>().OnDie += OnDie;
        GetComponent<Health>().OnDamage += OnDamage;
    }
    #endregion


    private IEnumerator StartFireBehaviour()
    {
        // don't make dmg until game isn't launched
        yield return new WaitUntil(() => GameManager.State == GAME_STATE.PLAYING);

        // wait X sec until the fire becomes harmful
        yield return new WaitForSeconds(_harmlessTime);

        // becomes a damaging fire
        ChangeState(STATE.RED);
        StartCoroutine("DamageBehaviour"); // launch dmg loop
    }

    private IEnumerator DamageBehaviour() 
    {
        float frequencyTime = 0;

        // make dmg every x time constantly
        while (true)
        {
            frequencyTime += Time.deltaTime;

            // it's dmg time !
            if(frequencyTime >= _dmgFrequency)
            {
                MakeDamage();

                frequencyTime = 0;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void ChangeState(STATE state)
    {
        _state = state;

        switch (state)
        {
            case STATE.YELLOW:
                myMesh.material = _yellowMat; // change first mat by default, to edit if multiple mat ?
                break;
            case STATE.RED:
                myMesh.material = _redMat; // change first mat by default, to edit if multiple mat ?
                break ;
            default:
                return;
        }
    }

    private void MakeDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _dmgRadius, _fireDamageable);

        //// make dmg on each fire damageable object that collides with this fire
        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.TryGetComponent<Health>(out var h))
            {
                if (h.Weakness == TYPE.FIRE)
                    h.TakeDamage(_redDmg);
            }
        }
    }


    private void OnDie()
    {
        // TUTO/FIRST FIRE CONTROLLER DESTROY => launch game
        if (GameManager.State == GAME_STATE.TUTO)
            GameManager.Instance.LaunchGame();

        // -- VISUALS --
        //_deathVFX.SetActive(true);
        _fireVFX.SetActive(false);
        _deathPS?.Play();

        // -- DESTROY --
        Destroy(gameObject,2); // hard code time for now
    }

    private void OnDamage(int dmg)
    {
        _damagePS?.Play(); // play dmg particle system
    }


    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _dmgRadius);
    }

    #endregion
}
