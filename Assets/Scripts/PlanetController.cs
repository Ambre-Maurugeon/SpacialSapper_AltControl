
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] Renderer _lifeRenderer;

    // ui life advance
    List<Material> _lifeMaterials= new();
    private float _lifeAdvance = 0;

    private bool _lifeIsDrecreasing = false;

    // health
    Health _healthComponent;
    private float _initHealth;


    #region Main
    private void Awake()
    {
        InitMyself();
    }

    private void Start()
    {
        InitMyselfWithOthers();
    }
    #endregion

    #region Init

    private void InitMyself()
    {
        _healthComponent = GetComponent<Health>();
        if( _healthComponent == null ) Debug.LogError("Health Component didn't find on "+ gameObject.name);
    }

    private void InitMyselfWithOthers()
    {
        // health
        _healthComponent.OnDie += OnDie;
        _healthComponent.OnDamage += OnDamage;

        _initHealth = _healthComponent.MyHealth;

        // water jet
        _lifeRenderer.GetMaterials(_lifeMaterials); // init material list
        ResetLifeAdvance(); // start without water jet
    }
   

    private IEnumerator DecreaseAdvanceProgressively()
    {
        _lifeIsDrecreasing = true;

        while (_lifeAdvance > Mathf.Clamp01(_healthComponent.MyHealth / _initHealth))
        {
            float targetAdvance = Mathf.Clamp01(_healthComponent.MyHealth / _initHealth);

            _lifeAdvance = Mathf.MoveTowards(
                _lifeAdvance,
                targetAdvance,
                Time.deltaTime / 2f
            );

            ApplyAdvanceOnLife();

            yield return null;
        }

        _lifeAdvance = Mathf.Clamp01(_healthComponent.MyHealth / _initHealth);
        ApplyAdvanceOnLife();

        _lifeIsDrecreasing = false;
    }


    private void ResetLifeAdvance()
    {
        _lifeAdvance = 1; 
        ApplyAdvanceOnLife();
    }
    private void ApplyAdvanceOnLife()
    {
        // apply advance value in mats
        for (int i = 0; i < _lifeMaterials.Count; i++)
        {
            _lifeMaterials[i].SetFloat("_advance", _lifeAdvance);
        }
    }


    private void OnDamage(int dmg)
    {
        if (!_lifeIsDrecreasing)
            StartCoroutine("DecreaseAdvanceProgressively");
    }

    private void OnDie()
    {
        if (GameManager.Instance.State == GAME_STATE.PLAYING)
            GameManager.Instance.LaunchDefeat();
    }
    #endregion


}
