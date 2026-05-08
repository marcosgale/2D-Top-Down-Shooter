using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField, Tooltip("Objects within this radius receive damage.")]
    private float _radius = 5.0f;

    [SerializeField, Tooltip("Must be lower than or equal to radius. If an object is within this radius, the entity receives full damage.")]
    private float _maxImpactRadius = 1.0f;

    [SerializeField, Tooltip("The damage that objects receive when they are within max impact radius.")]
    private float _baseDamage = 100.0f;

    [SerializeField, Tooltip("How quickly the damage is weakened if the damaged object is outside the max impact radius.")]
    private float _damageFalloff = 1.0f;

    [SerializeField, Tooltip("Layer mask used during overlap check.")]
    private LayerMask _overlapLayerMask = -1;

    [SerializeField, Tooltip("Layer mask used during wall check.")]
    private LayerMask _wallCheckLayerMask = -1;

    [SerializeField]
    private GameObject _explosionVfxPrefab;

    [SerializeField]
    private AudioClipGroup _explosionClipGroup;

    [HideInInspector]
    public GameObject _instigator;

    private void OnValidate()
    {
        if (_radius < 0.0f)
            _radius = 1.0f;

        if (_maxImpactRadius > _radius)
            _maxImpactRadius = _radius;

        if (_damageFalloff < 1.0f)
            _damageFalloff = 1.0f;
    }

    private void Start()
    {
        // We need to keep track of damaged objects during the loop to ensure that
        // the same object does not receive damage more than once.
        List<IDamageable> damagedObjects = new List<IDamageable>();

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = _overlapLayerMask;

        // Get overlapping colliders.
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        int overlappingCollidersCount = Physics2D.OverlapCircle(transform.position, _radius, contactFilter, overlappingColliders);

        // Inflict damage to objects within the explosion.
        for (int i = 0; i < overlappingCollidersCount; ++i)
        {
            IDamageable damageableObject = overlappingColliders[i].GetComponentInParent<IDamageable>();
            if (damageableObject == null)
                continue;

            // This object has already taken damage from the explosion.
            if (damagedObjects.Contains(damageableObject))
                continue;

            // Object won't take damage if it is behind a wall.
            if (IsObjectBehindWall(overlappingColliders[i]))
                continue;

            float damage = GetDamage(overlappingColliders[i]);
            damageableObject.OnTakeDamage(damage, overlappingColliders[i].transform.position, _instigator);

            damagedObjects.Add(damageableObject);
        }

        // Play the explosion particle effect.
        if (_explosionVfxPrefab)
        {
            GameObject explosionVfxInstance = Instantiate(_explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        // Play the explosion sound effect.
        AudioSourcePool.Instance.PlayOneShot(transform.position, _explosionClipGroup.GetRandomAudioClip());

        Destroy(gameObject);
    }

    private float GetDamage(Collider2D target)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget > _radius)
            return 0.0f;

        if (distanceToTarget <= _maxImpactRadius)
            return _baseDamage;

        float t = (distanceToTarget - _maxImpactRadius) / (_radius - _maxImpactRadius);

        float damage = Mathf.Lerp(_baseDamage, 0.0f, Mathf.Pow(t, _damageFalloff));

        return damage;
    }

    private bool IsObjectBehindWall(Collider2D target)
    {
        Vector3 directionToTarget = target.transform.position - transform.position;

        RaycastHit2D hitResult = Physics2D.Raycast(transform.position, directionToTarget.normalized, directionToTarget.magnitude, _wallCheckLayerMask);
        if (hitResult.collider && hitResult.collider != target)
            return true;

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxImpactRadius);
    }
}
