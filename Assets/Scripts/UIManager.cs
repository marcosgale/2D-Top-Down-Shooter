using UnityEngine;
using UnityEngine.UIElements;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager Instance;
    [SerializeField] private Sprite defaultWeaponSprite;
    [SerializeField] private Sprite defaultConsumableSprite;

    private VisualElement root;
    private VisualElement weaponSlot;
    private VisualElement consumableSlot;
    private VisualElement ammoBar;

    void Awake()
    {
        Instance = this;
        Debug.Log("=== WeaponUIManager Awake ===");

        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("UIDocument component not found on WeaponUIManager!");
            return;
        }

        root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root visual element is null!");
            return;
        }

        weaponSlot = root.Q<VisualElement>("WeaponSlot");
        if (weaponSlot == null)
            Debug.LogError("WeaponSlot UI element not found in UXML!");
        else
            Debug.Log($"WeaponSlot found! Current background: {weaponSlot.style.backgroundImage.value}");

        consumableSlot = root.Q<VisualElement>("ConsumableSlot");
        if (consumableSlot == null)
            Debug.LogWarning("ConsumableSlot UI element not found in UXML!");

        // find the ammo bar inside the weapon slot
        ammoBar = weaponSlot?.Q<VisualElement>("AmmoBar");
        if (ammoBar == null)
            Debug.LogWarning("AmmoBar UI element not found!");

        if (defaultWeaponSprite != null)
            SetWeaponIcon(defaultWeaponSprite);

        if (defaultConsumableSprite != null)
            SetConsumableIcon(defaultConsumableSprite);

        // initialize ammo bar as full (100%)
        if (ammoBar != null)
            ammoBar.style.width = Length.Percent(100);

        Debug.Log("=== WeaponUIManager Awake Complete ===");
    }

    public void SetWeaponIcon(Sprite newWeapon)
    {
        Debug.Log($"=== SetWeaponIcon called ===");
        Debug.Log($"Sprite: {(newWeapon != null ? newWeapon.name : "NULL")}");
        Debug.Log($"WeaponSlot: {(weaponSlot != null ? "Found" : "NULL")}");

        if (newWeapon != null && weaponSlot != null)
        {
            weaponSlot.style.backgroundImage = new StyleBackground(newWeapon);
            weaponSlot.style.unityBackgroundImageTintColor = Color.white;
            Debug.Log($"SUCCESS! Set weapon icon to: {newWeapon.name}");
            Debug.Log($"Sprite texture size: {newWeapon.texture.width}x{newWeapon.texture.height}");
            Debug.Log($"Background image after set: {weaponSlot.style.backgroundImage.value}");
            Debug.Log($"WeaponSlot computed style - width: {weaponSlot.resolvedStyle.width}, height: {weaponSlot.resolvedStyle.height}");
            Debug.Log($"WeaponSlot visible: {weaponSlot.resolvedStyle.visibility}, display: {weaponSlot.resolvedStyle.display}");
        }
        else
        {
            if (newWeapon == null)
                Debug.LogWarning("SetWeaponIcon called with null sprite!");
            if (weaponSlot == null)
                Debug.LogWarning("WeaponSlot UI element not found!");
        }
    }

    public void SetConsumableIcon(Sprite newConsumable)
    {
        if (newConsumable != null)
            consumableSlot.style.backgroundImage = new StyleBackground(newConsumable.texture);
    }

    // NEW: controls the ammo bar fill level (0–1)
    public void SetAmmo(float percentage)
    {
        if (ammoBar != null)
        {
            percentage = Mathf.Clamp01(percentage);
            ammoBar.style.width = Length.Percent(percentage * 100);
        }
    }

    /// <summary>
    /// Updates the ammo display with current clip and reserve ammo counts
    /// <summary>
    /// Updates the ammo display with current clip and reserve ammo counts
    /// </summary>
    public void UpdateAmmoDisplay(int clipAmmo, int reserveAmmo)
    {
        // This method is kept for compatibility but not used with legacy UI
        // The legacy AmmoCount label is updated elsewhere
    }

    /// <summary>
    /// Sets the currently equipped weapon icon (same as SetWeaponIcon)
    /// </summary>
    public void SetCurrentWeaponIcon(Sprite weaponSprite)
    {
        SetWeaponIcon(weaponSprite);
    }
}