using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public const string STATNAME_CLIPSIZE = "Clip Size";
    public const string STATNAME_MAXCLIPSIZE = "Max Clip Size";
    public const string STATNAME_RESERVEDAMMOCOUNT = "Reserved Ammo Count";
    public const string STATNAME_MAXAMMOCOUNT = "Max Ammo Count";
    public const string STATNAME_BASEDAMAGE = "Base Damage";
    public const string STATNAME_RELOADTIME = "Reload Time";
    public const string STATNAME_FIRERATE = "Fire Rate";
    public const string STATNAME_SPRAYANGLE = "Spray Angle";

    [HideInInspector] public bool IsGunTriggerHeld = false;
    [HideInInspector] public bool ShouldReload = false;

    [SerializeField, Tooltip("Icon sprite for this weapon to display in UI")]
    private Sprite _weaponIcon;
    public Sprite WeaponIcon => _weaponIcon;

    [SerializeField]
    private StatComponent _statComponent;
    public StatComponent StatComponent => _statComponent;

    [SerializeField]
    private AudioClipGroup _fireClipGroup;
    public AudioClipGroup FireClipGroup => _fireClipGroup;

    [SerializeField]
    private AudioClipGroup _reloadClipGroup;
    public AudioClipGroup ReloadClipGroup => _reloadClipGroup;

    public float ClipSize
    {
        get => StatComponent.GetValue(STATNAME_CLIPSIZE);
        set => StatComponent.SetValue(STATNAME_CLIPSIZE, value);
    }

    public float MaxClipSize => StatComponent.GetValue(STATNAME_MAXCLIPSIZE);

    public float ReservedAmmoCount
    {
        get => StatComponent.GetValue(STATNAME_RESERVEDAMMOCOUNT);
        set => StatComponent.SetValue(STATNAME_RESERVEDAMMOCOUNT, value);
    }

    public float MaxAmmoCount => StatComponent.GetValue(STATNAME_MAXAMMOCOUNT);

    public float BaseDamage => StatComponent.GetValue(STATNAME_BASEDAMAGE);

    public float ReloadTime => StatComponent.GetValue(STATNAME_RELOADTIME);

    public float FireRate => StatComponent.GetValue(STATNAME_FIRERATE);

    public float SprayAngle => StatComponent.GetValue(STATNAME_SPRAYANGLE);

    protected virtual void OnValidate()
    {
        if (!_statComponent)
            _statComponent = GetComponent<StatComponent>();
    }

    public void AddAmmo(float amount)
    {
        if (IsAmmoFull() == true)
        {
            return;
        }
        else
        {
            ReservedAmmoCount += amount;
            ReservedAmmoCount = Mathf.Clamp(ReservedAmmoCount, 0, MaxAmmoCount - ClipSize);
        }    
    }
    public void RemoveAmmo(float amount)
    {
        ReservedAmmoCount -= amount;
    }

    public bool IsAmmoFull()
    {
        return (ReservedAmmoCount + ClipSize) >= MaxAmmoCount;
    }
}
