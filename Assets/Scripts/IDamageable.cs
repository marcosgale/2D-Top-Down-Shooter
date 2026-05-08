using UnityEngine;

public interface IDamageable
{
    void OnTakeDamage(float damage, Vector2 impactPoint, GameObject instigator);
}
