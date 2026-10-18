using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    #region Fields

    [SerializeField] int _initHealth = 100;
    [SerializeField] TYPE _weakness = TYPE.NONE;
    [SerializeField] UnityEvent _onDie;

    private int _health;
    public TYPE Weakness => _weakness;

    #endregion Fields

    #region Main
    private void Awake()
    {
        _health = _initHealth;
    }

    #endregion

    public void TakeDamage(int dmg)
    {
        if (dmg < 0) return;

        if (_health <= 0) return;

        _health = Mathf.Max(0, _health-dmg);

        Debug.Log("takedmg, life : " + _health);

        // UpdateVisuals();

        if (_health == 0)
        {
            Debug.Log("He t'es mort tdc");
            _onDie?.Invoke();
        }
    }

    public void Heal(int heal)
    {
        if (heal < 0) return;

        _health = Mathf.Min(_initHealth,_health - heal);

        Debug.Log("heal, life : " + _health);

        // UpdateVisuals();
    }

    private void UpdateVisuals()
    {

    }
}
