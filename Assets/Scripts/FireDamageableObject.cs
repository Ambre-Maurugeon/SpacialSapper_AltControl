using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireDamageableObject : MonoBehaviour
{
    private float _initHealth;
    private float _currentHealth;
    private float _currentHealthPercent = 1;

    private float _speed = 40;
    private bool _isDead = false;
    private bool _isDecreasing = false;

    #region Main
    private void Start()
    {
        InitMyselfWithOthers();
    }
    #endregion

    #region Init
    private void InitMyselfWithOthers()
    {
        // health
        GetComponent<Health>().OnDie += OnDie;
        GetComponent<Health>().OnDamage += OnDamage;

        _initHealth = GetComponent<Health>().MyHealth;
        _currentHealth = _initHealth;
    }
    #endregion

    // CODE DEGUEU MAIS EN TEST
    private void OnDamage()
    {
        if(!_isDecreasing)
            StartCoroutine("DecreaseHealthProgressively");
    }

    private IEnumerator DecreaseHealthProgressively()
    {
        _isDecreasing = true ;

        float targetHealth = GetComponent<Health>().MyHealth;

        // -- BAD BUT TO TEST WITH GA ASSETS --
        //float frequency = 3;
        //float dmgToDeduct = _currentHealth - targetHealth;
        //float frequency2 = dmgToDeduct / frequency;
        //_speed = frequency2;

        while (_currentHealth > targetHealth)
        {
            _currentHealth -= Time.deltaTime * _speed; // get percent

            _currentHealthPercent = _currentHealth / _initHealth;

            yield return new WaitForEndOfFrame();
        }
        _currentHealth = Mathf.Clamp(targetHealth, 0, _initHealth);

        if (_currentHealth <= 0)
            _isDead = true;

        _isDecreasing = false;

    }

    private void OnDie()
    {
        StartCoroutine("Die");
    }

    private IEnumerator Die()
    {
        yield return new WaitUntil(() => _isDead);

        // activate cendres object
        Destroy(gameObject); // for now
    }
}
