using System.Security.Cryptography;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IDamageable
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private StatComponent _statComponent;

    [SerializeField]
    private StatDefinition _healthStat;

    private void OnValidate()
    {
        if (!_statComponent)
            _statComponent = GetComponent<StatComponent>();
    }

    public void OnTakeDamage(float amount, Vector2 impactPoint, GameObject instigator)
    {
        float currentHealth = _statComponent.GetValue(_healthStat);
        currentHealth -= amount;
        _statComponent.SetValue(_healthStat, currentHealth);

        if (currentHealth <= 0.0f)
            Explode();
    }

    public void Explode()
    {
        //SoundEffectManager.Play("BarrelExplosion");
        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
