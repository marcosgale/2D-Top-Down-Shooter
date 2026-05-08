using UnityEngine;

public class StatsComponent : MonoBehaviour
{
    public delegate void OnHealthReachesZero();
    public event OnHealthReachesZero onHealthReachesZero;

    [SerializeField]
    private float _health = 100.0f;

    [SerializeField]
    private float _maxHealth = 100.0f;

    public float GetHealth()
    {
        return _health;
    }

    public void SetHealth(float health)
    {
        if (health > _maxHealth)
            health = _maxHealth;

        _health = health;

        if (_health <= 0.0f)
            onHealthReachesZero.Invoke();
    }
}
