using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character, IDamageable
{
    private static PlayerCharacter _instance;
    public static PlayerCharacter Instance => _instance;

    [Header("Input")]

    [SerializeField]
    private PlayerInGameInput _playerInput;

    [Header("Shooting")]

    [SerializeField]
    private Transform _projectileSpawnTransform;

    [Header("Melee")]

    [SerializeField]
    private int _meleeRaycastCount = 10;

    [SerializeField]
    private float _meleeSwingAngle = 45.0f;

    [SerializeField]
    private float _meleeRaycastLength = 2.0f;

    [SerializeField]
    private float _meleeDamage = 50.0f;

    [SerializeField]
    private float _meleePushBackForce = 5.0f;

    [SerializeField]
    private float _maxMeleeCooldown = 1.0f;

    private float _meleeCooldown = 0.0f;

    [Header("Grenade")]

    [SerializeField]
    private GameObject _grenadePrefab;

    [SerializeField]
    private float _grenadeLaunchVelocity = 100f;

    [Header("GunAttachment")]
    [SerializeField]
    private Transform _gunAttachmentTransform;

    private Weapon _currentWeapon;
    public Weapon CurrentWeapon => _currentWeapon;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!_playerInput)
            _playerInput = GetComponent<PlayerInGameInput>();
    }

    private void OnEnable()
    {
        Debug.Assert(_instance == null, "A player character has already existed in the scene!");
        _instance = this;

        _currentWeapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        // Only the the input values if the player character is still alive.
        if (IsAlive)
        {
            // Read the input for player movement.
            if (CharacterMovement is PlayerCharacterMovement playerMovement)
            {
                playerMovement.MoveInput = _playerInput.InputData.MoveInput;
                playerMovement.LookInput = _playerInput.InputData.LookInput;
                playerMovement.IsSprinting = _playerInput.InputData.IsSprintButtonPressed;
            }

            // Handle firing and reloading.
            if (_currentWeapon)
            {
                _currentWeapon.IsGunTriggerHeld = _playerInput.InputData.IsFireButtonPressed;
                _currentWeapon.ShouldReload = _playerInput.InputData.IsReloadButtonPressed;
            }

            // Handle melee attack.
            if (_playerInput.InputData.IsMeleeButtonPressed && _meleeCooldown <= 0.0f)
            {
                MeleeAttack();
                _meleeCooldown = _maxMeleeCooldown;
            }
            _meleeCooldown -= Time.deltaTime;

            // Handle grenade attack.
            if (_playerInput.InputData.IsThrowGrenadeButtonPressed)
                ThrowGrenade();
        }

        // Reset all the input values if the player character dies.
        else
        {
            if (CharacterMovement is PlayerCharacterMovement playerMovement)
            {
                playerMovement.MoveInput = Vector2.zero;
                playerMovement.LookInput = Vector2.zero;
                playerMovement.IsSprinting = false;
            }

            if (_currentWeapon)
                UnequipWeapon();
        }
    }

    public void MeleeAttack()
    {
        // Calculate the origin of the melee raycasts.
        Vector3 meleeStartPosition = _projectileSpawnTransform.position;
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

    public void ThrowGrenade()
    {
        float GrenadeCount = StatComponent.GetValue("GrenadeCount");
        if (GrenadeCount <= 0)
            return;

        GameObject grenadeGameObject = Instantiate(_grenadePrefab, _projectileSpawnTransform.position, _projectileSpawnTransform.rotation);
        Debug.Assert(grenadeGameObject, "grenadeGameObject is null! The grenade prefab may not be set.");

        // ?? Play grenade throw sound
        SoundEffectManager.Play("GrenadeThrow");

        Grenade grenade = grenadeGameObject.GetComponent<Grenade>();
        StatComponent.SetValue("GrenadeCount", GrenadeCount - 1);
        Debug.Assert(grenade, "The spawned object doesn't have the Grenade component!");

        Vector3 launchVelocity = _grenadeLaunchVelocity * transform.right;
        grenade.Rigidbody.linearVelocity = launchVelocity;
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (_gunAttachmentTransform == null)
        {
            Debug.LogError("Gun attachment transform is not assigned!");
            return;
        }

        if (_currentWeapon)
            Destroy(_currentWeapon.gameObject);

        GameObject weaponGameObject = Instantiate(weaponPrefab, _gunAttachmentTransform);
        
        // Ensure the weapon is active
        weaponGameObject.SetActive(true);
        
        _currentWeapon = weaponGameObject.GetComponent<Weapon>();

        if (_currentWeapon == null)
        {
            Debug.LogError($"The instantiated weapon '{weaponGameObject.name}' does not have a Weapon component!");
        }
        else
        {
            Debug.Log($"Weapon instantiated: {weaponGameObject.name}, Active: {weaponGameObject.activeInHierarchy}");
        }
    }
public void UnequipWeapon()
    {
        if (_currentWeapon)
            Destroy(_currentWeapon.gameObject);
        _currentWeapon = null;
    }
    public override void OnHealthReachesZero()
    {
        CharacterMovement.ShouldUpdatePosition = false;
        CharacterMovement.ShouldUpdateRotation = false;

        AudioSourcePool.Instance.PlayOneShot(transform.position, DeathClipGroup.GetRandomAudioClip());
        Debug.Log("Player has died!");
    }

    public void AddAmmo(float ammoAmount)
    {
        _currentWeapon.AddAmmo(ammoAmount);
    }
    public bool IsAmmoFull()
    {
        if (_currentWeapon == null)
            return true;
        return _currentWeapon.IsAmmoFull();
    }
    public void AddGrenades(float grenadeAmount)
    {
        float currentGrenadeCount = StatComponent.GetValue("GrenadeCount");
        StatComponent.SetValue("GrenadeCount", currentGrenadeCount + grenadeAmount);
    }

}
