using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TestEquipAndUnEquip : MonoBehaviour
{
    private static TestEquipAndUnEquip _instance;

    [SerializeField]
    public GameObject currentWeapon;

    [SerializeField]
    public StatComponent weaponStats;

    [SerializeField]
    public bool hasRifle = true;
    [SerializeField]
    public bool hasShotgun = false;
    [SerializeField]
    public bool hasPistol = false;
    [SerializeField]
    public bool hasRPG = false;

    [SerializeField]
    GameObject shotgunprefab;
    [SerializeField]
    GameObject RPGprefab;
    [SerializeField]
    GameObject pistolprefab;
    [SerializeField]
    GameObject rifleprefab;

    [SerializeField]
    PlayerCharacter player;

    [SerializeField, Tooltip("Reference to the Previous weapon input action")]
    private InputActionReference _inputActionPrevious;

    [SerializeField, Tooltip("Reference to the Next weapon input action")]
    private InputActionReference _inputActionNext;

    private Dictionary<GameObject, Dictionary<string, float>> _weaponStatSnapshots = new Dictionary<GameObject, Dictionary<string, float>>();

    // Track current weapon index for cycling
    private int _currentWeaponIndex = 0;

    // List of stat keys we want to persist per weapon
    private readonly string[] _persistedStatKeys = new[]
    {
        Weapon.STATNAME_CLIPSIZE,
        Weapon.STATNAME_MAXCLIPSIZE,
        Weapon.STATNAME_RESERVEDAMMOCOUNT,
        Weapon.STATNAME_MAXAMMOCOUNT,
        Weapon.STATNAME_BASEDAMAGE,
        Weapon.STATNAME_RELOADTIME,
        Weapon.STATNAME_FIRERATE,
        Weapon.STATNAME_SPRAYANGLE
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Singleton pattern - persist across scenes
        if (_instance != null && _instance != this)
        {
            // Instance already exists, transfer weapon states to it and destroy this duplicate
            _instance.hasRifle = this.hasRifle || _instance.hasRifle;
            _instance.hasShotgun = this.hasShotgun || _instance.hasShotgun;
            _instance.hasPistol = this.hasPistol || _instance.hasPistol;
            _instance.hasRPG = this.hasRPG || _instance.hasRPG;
            
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        
        Debug.Log("=== TestEquipAndUnEquip Start ===");
        Debug.Log($"rifleprefab assigned: {rifleprefab != null}");
        Debug.Log($"player assigned: {player != null}");
        
        if (player == null)
        {
            Debug.LogError("Player is not assigned in TestEquipAndUnEquip! Please assign it in the Inspector.");
            return;
        }
        
        if (rifleprefab == null)
        {
            Debug.LogError("Rifle prefab is not assigned in TestEquipAndUnEquip! Please assign it in the Inspector.");
            return;
        }

        // Equip the rifle
        Debug.Log("Attempting to equip rifle at start...");
        player.EquipWeapon(rifleprefab);
        UpdateWeaponUI(rifleprefab);
        
        // Verify it was equipped
        if (player.CurrentWeapon != null)
        {
            Debug.Log($"SUCCESS: Rifle equipped - {player.CurrentWeapon.gameObject.name}");
            currentWeapon = player.CurrentWeapon.gameObject;
            
            // Debug weapon visual state
            DebugWeaponState(currentWeapon);
            
            StartCoroutine(SaveStatsNextFrame());
        }
        else
        {
            Debug.LogError("FAILED: Rifle was not equipped! Check PlayerCharacter._gunAttachmentTransform in Inspector.");
        }
    }

    private System.Collections.IEnumerator SaveStatsNextFrame()
    {
        yield return null; // Wait one frame
        SaveCurrentWeaponStats();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        currentWeapon = player.CurrentWeapon?.gameObject;

        if (weaponStats != null) // added null check for weaponStats
        {
            Debug.Log($"Weapon stats loaded for: {currentWeapon?.name}");
        }
    }

    void CheckInput()
    {
        // Debug to give all weapons
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            hasRifle = true;
            hasShotgun = true;
            hasPistol = true;
            hasRPG = true;
            Debug.Log("All weapons unlocked.");
        }

        // D-pad / Input Action cycling
        if (_inputActionPrevious != null && _inputActionPrevious.action.WasPressedThisFrame())
        {
            CycleToPreviousWeapon();
        }
        if (_inputActionNext != null && _inputActionNext.action.WasPressedThisFrame())
        {
            CycleToNextWeapon();
        }

        // Original number key inputs
        if (hasRifle && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            Debug.Log("Attempting to equip rifle...");
            if (player != null && rifleprefab != null)
            {
                SaveCurrentWeaponStats();
                player.EquipWeapon(rifleprefab);
                RestoreStatsForPrefab(rifleprefab);
                UpdateWeaponUI(rifleprefab);
                _currentWeaponIndex = 0; // Rifle is index 0
            }
            else
            {
                Debug.LogWarning("Player or rifle prefab is not assigned.");
            }
        }
        if (hasShotgun && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            if (player != null && shotgunprefab != null)
            {
                SaveCurrentWeaponStats();
                player.EquipWeapon(shotgunprefab);
                RestoreStatsForPrefab(shotgunprefab);
                UpdateWeaponUI(shotgunprefab);
                _currentWeaponIndex = 1; // Shotgun is index 1
            }
        }
        if (hasPistol && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            if (player != null && pistolprefab != null)
            {
                SaveCurrentWeaponStats();
                player.EquipWeapon(pistolprefab);
                RestoreStatsForPrefab(pistolprefab);
                UpdateWeaponUI(pistolprefab);
                _currentWeaponIndex = 2; // Pistol is index 2
            }
        }
        if (hasRPG && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)))
        {
            if (player != null && RPGprefab != null)
            {
                SaveCurrentWeaponStats();
                player.EquipWeapon(RPGprefab);
                RestoreStatsForPrefab(RPGprefab);
                UpdateWeaponUI(RPGprefab);
                _currentWeaponIndex = 3; // RPG is index 3
            }
        }
    }

    // Save the stat values of the currently equipped weapon into the snapshot dictionary,
    // keyed by the matching prefab (if one of the known prefabs matches the current weapon instance).
    void SaveCurrentWeaponStats()
    {
        if (player == null) return;
        Weapon CurrentWeapon = player.CurrentWeapon;
        if (CurrentWeapon == null) return;

        StatComponent cwStatComponent = CurrentWeapon.StatComponent;
        if (cwStatComponent == null)
        {
            Debug.LogWarning("StatComponent is null for the current weapon.");
            return;
        }

        Debug.Log($"Saving stats for weapon: {CurrentWeapon.name}");
        var snapshot = new Dictionary<string, float>();
        foreach (var key in _persistedStatKeys)
        {
            if (cwStatComponent.TryGetValue(key, out float value))
            {
                snapshot[key] = value;
                Debug.Log($"  Saved {key}: {value}");
            }
            else
            {
                Debug.LogWarning($"Stat '{key}' not found in StatComponent. Make sure the weapon prefab has this stat defined in _initialStats.");
            }
        }
        GameObject prefab = GetPrefabForCurrentWeapon(CurrentWeapon);
        if (prefab != null)
        {
            _weaponStatSnapshots[prefab] = snapshot;
            Debug.Log($"Saved stats for prefab '{prefab.name}'");
        }
        else
        {
            Debug.LogWarning($"Could not determine prefab for current weapon instance '{CurrentWeapon.name}'. Stats not saved.");
        }
    }

    // Restore saved stat values (if present) into the currently equipped weapon after equipping the given prefab.
    void RestoreStatsForPrefab(GameObject prefab)
    {
        if (player == null || prefab == null) return;
        if (!_weaponStatSnapshots.TryGetValue(prefab, out var snapshot))
        {
            Debug.LogWarning($"No saved stats found for prefab '{prefab.name}'");
            return;
        }

        Weapon currentWeapon = player.CurrentWeapon;
        if (currentWeapon == null) return;

        StatComponent cwStatComponent = currentWeapon.StatComponent;
        if (cwStatComponent == null) return;

        foreach (var kvp in snapshot)
        {
            cwStatComponent.SetValue(kvp.Key, kvp.Value);
        }

        if (cwStatComponent.TryGetValue(Weapon.STATNAME_CLIPSIZE, out float clip))
        {
            currentWeapon.ClipSize = clip;
        }
        if (cwStatComponent.TryGetValue(Weapon.STATNAME_RESERVEDAMMOCOUNT, out float reserved))
        {
            currentWeapon.ReservedAmmoCount = reserved;
        }

        Debug.Log($"Restored stats for prefab '{prefab.name}' to instance '{currentWeapon.name}'");
    }

    // Try to determine which prefab corresponds to the current weapon instance.
    // We compare names: prefab.name vs currentWeapon.name (which often becomes "PrefabName(Clone)").
    GameObject GetPrefabForCurrentWeapon(Weapon weaponInstance)
    {
        if (weaponInstance == null) return null;
        string instanceName = weaponInstance.gameObject.name;

        Debug.Log($"Checking prefab for weapon instance: {instanceName}");

        if (rifleprefab != null && instanceName.StartsWith(rifleprefab.name))
            return rifleprefab;
        if (shotgunprefab != null && instanceName.StartsWith(shotgunprefab.name))
            return shotgunprefab;
        if (pistolprefab != null && instanceName.StartsWith(pistolprefab.name))
            return pistolprefab;
        if (RPGprefab != null && instanceName.StartsWith(RPGprefab.name))
            return RPGprefab;

        return null;
    }

    void DebugWeaponState(GameObject weapon)
    {
        Debug.Log("=== Weapon Debug Info ===");
        Debug.Log($"Weapon Position: {weapon.transform.position}");
        Debug.Log($"Weapon Local Position: {weapon.transform.localPosition}");
        Debug.Log($"Weapon Scale: {weapon.transform.localScale}");
        Debug.Log($"Weapon Active: {weapon.activeInHierarchy}");
        
        SpriteRenderer[] sprites = weapon.GetComponentsInChildren<SpriteRenderer>();
        Debug.Log($"SpriteRenderers found: {sprites.Length}");
        foreach (var sr in sprites)
        {
            Debug.Log($"  - Sprite: {sr.sprite?.name}, Enabled: {sr.enabled}, Sorting Layer: {sr.sortingLayerName}, Order: {sr.sortingOrder}");
        }
        
        MeshRenderer[] meshes = weapon.GetComponentsInChildren<MeshRenderer>();
        Debug.Log($"MeshRenderers found: {meshes.Length}");
        
        Debug.Log($"Children count: {weapon.transform.childCount}");
        for (int i = 0; i < weapon.transform.childCount; i++)
        {
            Transform child = weapon.transform.GetChild(i);
            Debug.Log($"  Child {i}: {child.name}, Active: {child.gameObject.activeInHierarchy}");
        }
    }

    /// <summary>
    /// Cycle to the next available weapon
    /// </summary>
    void CycleToNextWeapon()
    {
        // Build list of available weapons
        var availableWeapons = GetAvailableWeaponsList();
        if (availableWeapons.Count == 0)
        {
            Debug.LogWarning("No weapons available to cycle through!");
            return;
        }

        // Move to next weapon
        _currentWeaponIndex = (_currentWeaponIndex + 1) % availableWeapons.Count;
        EquipWeaponByIndex(availableWeapons, _currentWeaponIndex);
    }

    /// <summary>
    /// Cycle to the previous available weapon
    /// </summary>
    void CycleToPreviousWeapon()
    {
        // Build list of available weapons
        var availableWeapons = GetAvailableWeaponsList();
        if (availableWeapons.Count == 0)
        {
            Debug.LogWarning("No weapons available to cycle through!");
            return;
        }

        // Move to previous weapon
        _currentWeaponIndex--;
        if (_currentWeaponIndex < 0)
            _currentWeaponIndex = availableWeapons.Count - 1;
        
        EquipWeaponByIndex(availableWeapons, _currentWeaponIndex);
    }

    /// <summary>
    /// Get list of currently available weapon prefabs
    /// </summary>
    List<GameObject> GetAvailableWeaponsList()
    {
        var weapons = new List<GameObject>();
        if (hasRifle && rifleprefab != null) weapons.Add(rifleprefab);
        if (hasShotgun && shotgunprefab != null) weapons.Add(shotgunprefab);
        if (hasPistol && pistolprefab != null) weapons.Add(pistolprefab);
        if (hasRPG && RPGprefab != null) weapons.Add(RPGprefab);
        return weapons;
    }

    /// <summary>
    /// Equip a weapon from the available weapons list by index
    /// </summary>
    void EquipWeaponByIndex(List<GameObject> weaponList, int index)
    {
        if (index < 0 || index >= weaponList.Count) return;
        
        GameObject weaponToEquip = weaponList[index];
        if (player != null && weaponToEquip != null)
        {
            SaveCurrentWeaponStats();
            player.EquipWeapon(weaponToEquip);
            RestoreStatsForPrefab(weaponToEquip);
            UpdateWeaponUI(weaponToEquip);
            Debug.Log($"Cycled to weapon: {weaponToEquip.name}");
        }
    }

    /// <summary>
    /// Updates the UI to display the current weapon icon
    /// </summary>
    void UpdateWeaponUI(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;

        // Get the Weapon component from the prefab
        Weapon weaponComponent = weaponPrefab.GetComponent<Weapon>();
        if (weaponComponent == null)
        {
            Debug.LogWarning($"No Weapon component found on {weaponPrefab.name}");
            return;
        }

        // Update the UI with the weapon icon
        if (WeaponUIManager.Instance != null && weaponComponent.WeaponIcon != null)
        {
            WeaponUIManager.Instance.SetWeaponIcon(weaponComponent.WeaponIcon);
            Debug.Log($"Updated UI with weapon icon for {weaponPrefab.name}");
        }
        else
        {
            if (WeaponUIManager.Instance == null)
                Debug.LogWarning("WeaponUIManager.Instance is null! Make sure WeaponUIManager is in the scene.");
            if (weaponComponent.WeaponIcon == null)
                Debug.LogWarning($"Weapon icon not assigned for {weaponPrefab.name}. Assign it in the prefab's Inspector.");
        }
    }
}
