using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    public const int MAX_WEAPON_SLOTS = 10;

    private WeaponData[] _weaponSlots;

    private void Awake()
    {
        _weaponSlots = new WeaponData[MAX_WEAPON_SLOTS];
    }
}
