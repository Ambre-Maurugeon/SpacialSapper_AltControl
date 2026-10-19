using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlanetController : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider _healthSlider;
    [SerializeField] private float _speed = 20;

    Health _healthComponent;

    private float _initHealth;

    private bool _isDrecreasing = false;

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
        _healthSlider.value = _initHealth;
    }
   
    private void OnDamage(int dmg)
    {
        if (!_isDrecreasing)
            StartCoroutine("DecreaseSliderValueProgressively");
    }

    private IEnumerator DecreaseSliderValueProgressively()
    {
        _isDrecreasing = true;

        // slider value progressively reaches the health value
        while (_healthSlider.value > _healthComponent.MyHealth / _initHealth)
        {
            _healthSlider.value -= Time.deltaTime ;
            yield return new WaitForEndOfFrame();
        }

        _isDrecreasing = false;
    }

    private void OnDie()
    {
        //
    }
    #endregion


}
