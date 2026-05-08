using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        Shotgun,
        RPG,
        Rifle
    }

    [Header("Weapon Configuration")]
    [SerializeField, Tooltip("The type of weapon this pickup unlocks")]
    private WeaponType _weaponType;

    [Header("Weapon Icon to Show on UI")]
    public Sprite weaponIcon; // The icon that will appear in the UI

    [Header("Pickup Settings")]
    [SerializeField, Tooltip("Optional: Visual feedback particle effect")]
    private GameObject _pickupEffect;

    [SerializeField, Tooltip("Optional: Sound to play on pickup")]
    private AudioClip _pickupSound;

    [Header("Rendering Settings")]
    [SerializeField, Tooltip("Sorting layer name for the pickup sprite")]
    private string _sortingLayerName = "Default";

    [SerializeField, Tooltip("Order in layer (higher = renders on top)")]
    private int _orderInLayer = 10;

    private void Start()
    {
        // Ensure the pickup sprite renders above tilemaps
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = _sortingLayerName;
            spriteRenderer.sortingOrder = _orderInLayer;
        }

        // Also check child sprite renderers
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in childRenderers)
        {
            renderer.sortingLayerName = _sortingLayerName;
            renderer.sortingOrder = _orderInLayer;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has a PlayerCharacter component
        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter == null)
        {
            playerCharacter = other.GetComponentInParent<PlayerCharacter>();
        }

        if (playerCharacter == null)
            return;

        // Find the TestEquipAndUnEquip component on the player
        TestEquipAndUnEquip weaponManager = playerCharacter.GetComponent<TestEquipAndUnEquip>();
        if (weaponManager != null)
        {
            // Check if player already has this weapon
            if (!IsWeaponUnlocked(weaponManager))
            {
                // Unlock the weapon
                UnlockWeapon(weaponManager);
                Debug.Log($"Picked up {_weaponType}! Weapon unlocked.");
            }
            else
            {
                Debug.Log($"Player already has {_weaponType}");
            }
        }

        // Play pickup effects
        if (_pickupEffect != null)
        {
            Instantiate(_pickupEffect, transform.position, Quaternion.identity);
        }

        if (_pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(_pickupSound, transform.position);
        }

        // Destroy this pickup after it's collected
        Destroy(gameObject);
    }

    private bool IsWeaponUnlocked(TestEquipAndUnEquip weaponManager)
    {
        switch (_weaponType)
        {
            case WeaponType.Pistol:
                return weaponManager.hasPistol;
            case WeaponType.Shotgun:
                return weaponManager.hasShotgun;
            case WeaponType.RPG:
                return weaponManager.hasRPG;
            case WeaponType.Rifle:
                return weaponManager.hasRifle;
            default:
                return false;
        }
    }

    private void UnlockWeapon(TestEquipAndUnEquip weaponManager)
    {
        switch (_weaponType)
        {
            case WeaponType.Pistol:
                weaponManager.hasPistol = true;
                break;
            case WeaponType.Shotgun:
                weaponManager.hasShotgun = true;
                break;
            case WeaponType.RPG:
                weaponManager.hasRPG = true;
                break;
            case WeaponType.Rifle:
                weaponManager.hasRifle = true;
                break;
        }
    }
}
