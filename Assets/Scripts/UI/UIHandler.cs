using UnityEngine;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    [SerializeField]
    private float UIhealth;
   

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        UpdateHealthBar();
        UpdateAmmoCount();
        UpdateGrenadeCount();
        UpdateEnemyCount();
    }

    private void Update()
    {
        UpdateHealthBar();
        UpdateAmmoCount();
        UpdateGrenadeCount();
        UpdateEnemyCount();
    }


    float GetPlayerHealth()
    {
        if (PlayerCharacter.Instance != null && PlayerCharacter.Instance.StatComponent != null)
        {
            return PlayerCharacter.Instance.StatComponent.GetValue("Health");
        }
        return 0f;
    }

    float getPlayerMaxHealth()
    {
        // Replace with actual max health retrieval if available
        if (PlayerCharacter.Instance != null && PlayerCharacter.Instance.StatComponent != null)
        {
            return PlayerCharacter.Instance.StatComponent.GetValue("Max Health");
        }
        return 100f;
    }


    public void UpdateHealthBar()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        VisualElement healthBar = uiDocument.rootVisualElement.Q<VisualElement>("Bar");
        float current = GetPlayerHealth();
        float max = getPlayerMaxHealth();
        healthBar.style.width = Length.Percent(GetPlayerHealth() / getPlayerMaxHealth() * 60f);
        UIhealth = current;
    }

    float GetClipSize()
    {
        if (!PlayerCharacter.Instance)
            return 0;

        Weapon equippedWeapon = PlayerCharacter.Instance.CurrentWeapon;
        if (!equippedWeapon)
            return 0;

        float clipSize = equippedWeapon.StatComponent.GetValue(Weapon.STATNAME_CLIPSIZE);
        return clipSize;
    }

    float GetReservedAmmoCount()
    {
        if (!PlayerCharacter.Instance)
            return 0;

        Weapon equippedWeapon = PlayerCharacter.Instance.CurrentWeapon;
        if (!equippedWeapon)
            return 0;

        float reservedAmmoCount = equippedWeapon.StatComponent.GetValue(Weapon.STATNAME_RESERVEDAMMOCOUNT);
        return reservedAmmoCount;
    }

    public void UpdateAmmoCount()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        Label ammoLabel = uiDocument.rootVisualElement.Q<Label>("AmmoCount");
        ammoLabel.text = "Ammo: " + GetClipSize().ToString() + "/" + GetReservedAmmoCount().ToString();
    }

    public void UpdateGrenadeCount()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        Label grenadeLabel = uiDocument.rootVisualElement.Q<Label>("GrenadeCount");
        if (PlayerCharacter.Instance != null && PlayerCharacter.Instance.StatComponent != null)
        {
            float grenades = PlayerCharacter.Instance.StatComponent.GetValue("GrenadeCount");
            grenadeLabel.text = "Grenades: " + grenades.ToString();
        }
        else
        {
            grenadeLabel.text = "Grenades: 0";
        }
    }

    public void UpdateEnemyCount()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            return;
        }
        
        Label enemyLabel = uiDocument.rootVisualElement.Q<Label>("EnemyCount");
        if (enemyLabel == null)
        {
            return; // UI element doesn't exist yet
        }
        
        // Use TestCharacter's static instance count
        int enemyCount = TestCharacter.InstanceCount;
        
        if (enemyCount > 0)
        {
            enemyLabel.text = $"Enemies: {enemyCount}";
            enemyLabel.style.display = DisplayStyle.Flex;
        }
        else
        {
            enemyLabel.text = "Room Clear!";
            enemyLabel.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    /// Show splash message when trying to exit with enemies still in the room
    /// </summary>
    /// <param name="message">Optional custom message (defaults to standard message)</param>
    public void ShowRoomNotClearedMessage(string message = null)
    {
        if (RoomManager.Instance == null)
            return;

        if (!RoomManager.Instance.IsRoomCleared)
        {
            string displayMessage = message;
            if (string.IsNullOrEmpty(displayMessage))
            {
                displayMessage = $"Defeat all enemies before proceeding! ({RoomManager.Instance.EnemyCount} remaining)";
            }

            // Only use UI Toolkit version
            if (SplashMessageUIToolkit.Instance != null)
            {
                SplashMessageUIToolkit.Instance.ShowMessage(displayMessage);
            }
            else
            {
                Debug.LogWarning("No SplashMessageUIToolkit component found in scene!");
            }
        }
    }
}



