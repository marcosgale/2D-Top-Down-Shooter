using UnityEngine;

public class TestUIChange : MonoBehaviour
{
    public WeaponUIManager uiManager;   // Reference to your WeaponUIManager
    public Sprite newWeaponIcon;        // New weapon icon to test
    public Sprite newConsumableIcon;    // New consumable icon to test

    void Update()
    {
        // Press 1 to change weapon icon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            uiManager.SetWeaponIcon(newWeaponIcon);
            Debug.Log("Changed Weapon Icon!");
        }

        // Press 2 to change consumable icon
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            uiManager.SetConsumableIcon(newConsumableIcon);
            Debug.Log("Changed Consumable Icon!");
        }
    }
}
