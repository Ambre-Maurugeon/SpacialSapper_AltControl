using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _initHealth = 100;
    private int _health;

    #region Main
    private void Awake()
    {
        _health = _initHealth;
    }

    #endregion

    public void TakeDamage(int dmg)
    {
        if (dmg < 0) return;

        _health = Mathf.Max(0, _health-dmg);

        Debug.Log("takedmg, life : " + _health);

        if (_health == 0)
        {
            Debug.Log("He t'es mort tdc");
            Destroy(gameObject);
        }

        // UpdateVisuals();
    }

    private void UpdateVisuals()
    {

    }
}
