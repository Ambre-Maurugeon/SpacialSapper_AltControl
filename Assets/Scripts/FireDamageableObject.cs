
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamageableObject : MonoBehaviour
{
    // FOR GA
    // for mask offset
    private float _initHealth;
    private float _currentHealth;
    private float _maskOffsetAdvance = 1;

    [SerializeField] private float _speed = 40; // to edit if time => calculate speed using frequency
    private bool _isDead = false;
    private bool _isDecreasing = false;

    private List<Material> _materials = new();
    
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

        // damageable behaviour
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.GetMaterials(_materials);

        ResetMaskOffsetAdvance();
    }
    #endregion

    // CODE DEGUEU MAIS EN TEST
    private void OnDamage(int dmg)
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
            _currentHealth -= Time.deltaTime * _speed; 

            _maskOffsetAdvance = 1 - (_currentHealth / _initHealth); // get percent for GA use
            ApplyMaskOffsetOnMat();

            yield return new WaitForEndOfFrame();
        }
        _currentHealth = Mathf.Clamp(targetHealth, 0, _initHealth);

        if (_currentHealth <= 0)
            _isDead = true;

        _isDecreasing = false;

    }

    private void ResetMaskOffsetAdvance() // doesn't work ? why
    {
        _currentHealth = 0;
        _maskOffsetAdvance = 0; // to edit => do it progressively 
        ApplyMaskOffsetOnMat();
    }

    private void ApplyMaskOffsetOnMat()
    {
        // apply advance value in mats
        for (int i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetFloat("_maskoffset", _maskOffsetAdvance);
        }
    }

    private void OnDie()
    {
        StartCoroutine("Die");
    }

    private IEnumerator Die()
    {
        // decrease until death
        if (!_isDecreasing)
            StartCoroutine("DecreaseHealthProgressively");

        // wait until death
        yield return new WaitUntil(() => _isDead);

        // activate cendres object
        Destroy(gameObject); // for now
    }
}
