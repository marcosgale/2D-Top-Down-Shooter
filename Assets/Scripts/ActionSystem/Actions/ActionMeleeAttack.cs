using System.Collections.Generic;
using UnityEngine;

public class ActionMeleeAttack : Action
{
    [SerializeField]
    private Transform _meleeAttackOrigin;

    [SerializeField]
    private int _meleeRaycastCount = 10;

    [SerializeField]
    private float _meleeSwingAngle = 90.0f;

    [SerializeField]
    private float _meleeRaycastLength = 2.0f;

    [SerializeField]
    private float _meleeDamage = 50.0f;

    [SerializeField]
    private float _meleePushBackForce = 10000f;

    [SerializeField]
    private float _maxMeleeCooldown = 1.0f;

    private float _meleeCooldown = 0.0f;

    private void Update()
    {
        _meleeCooldown -= Time.deltaTime;
    }

    protected override bool CanBeExecuted()
    {
        return _meleeCooldown <= 0.0f;
    }

    protected override void OnExecuted()
    {
        // Reset the melee cooldown.
        _meleeCooldown = _maxMeleeCooldown;

        // Calculate the origin of the melee raycasts.
        Vector3 meleeStartPosition = _meleeAttackOrigin.position;
        meleeStartPosition.z = 0.0f;

        // Calculate the direction of the first melee raycast.
        Vector3 firstRayDirection = Quaternion.Euler(0.0f, 0.0f, -_meleeSwingAngle / 2.0f) * transform.right;

        // We need to keep track of hit colliders so that a damageable object does not take damage more than once.
        HashSet<Collider2D> damagedColliders = new HashSet<Collider2D>();

        for (int i = 0; i < _meleeRaycastCount; ++i)
        {
            float angle = _meleeSwingAngle / _meleeRaycastCount * i; // Get the angle between the current ray and the first ray.
            Vector3 rayDirection = Quaternion.Euler(0.0f, 0.0f, angle) * firstRayDirection;

            RaycastHit2D hitResult = Physics2D.Raycast(meleeStartPosition, rayDirection, _meleeRaycastLength);
            Debug.DrawLine(meleeStartPosition, meleeStartPosition + rayDirection * _meleeRaycastLength, Color.red, 3.0f, false);

            if (!hitResult.collider)
                continue;

            if (damagedColliders.Contains(hitResult.collider))
                continue;

            damagedColliders.Add(hitResult.collider);

            // Push the object back if the object has rigidbody.
            if (hitResult.rigidbody)
            {
                hitResult.rigidbody.AddForce(rayDirection * _meleePushBackForce);
            }

            // Inflict damage to the object if the object is damageable.
            IDamageable damageableObject = hitResult.collider.GetComponentInParent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.OnTakeDamage(_meleeDamage, hitResult.point, gameObject);
                Debug.Log($"Target hit: {(((Component)damageableObject).gameObject.name)}");
            }
        }
    }
}
