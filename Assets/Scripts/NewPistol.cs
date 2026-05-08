using System.Collections;
using UnityEngine;

/// <summary>
/// Semi-automatic pistol:
///  - 1 shot per click
///  - Holding the trigger does NOT keep firing
///  - Fire rate still respected (like CS pistols).
/// </summary>
public class NewPistol : Weapon
{
    [Header("Gun Settings")]
    [Tooltip("The bullet prefab to be instantiated when shooting.")]
    public GameObject bulletPrefab;

    [Tooltip("The transform where bullets will be spawned.")]
    public Transform firePoint;

    // --- Private Variables (Internal Game State) ---
    private float nextFireTime = 0f;
    private bool isReloading = false;

    // for semi-auto edge detection
    private bool wasTriggerHeldLastFrame = false;

    private void Update()
    {
        // --- SEMI-AUTO INPUT LOGIC ---
        // Only fire when the trigger goes from "not held" to "held" this frame
        bool triggerPressedThisFrame = IsGunTriggerHeld && !wasTriggerHeldLastFrame;

        if (triggerPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // Store for next frame
        wasTriggerHeldLastFrame = IsGunTriggerHeld;

        // Reload logic (unchanged)
        if (ShouldReload && !isReloading && ClipSize < MaxClipSize && ReservedAmmoCount > 0)
            StartCoroutine(Reload());
    }

    /// <summary> Instantiates a bullet, handles spray, and manages fire rate. </summary>
    private void Shoot()
    {
        // Return if the gun is reloading or out of ammo.
        if (isReloading || ClipSize <= 0)
            return;

        // Enforce fire rate here too (extra safety)
        if (Time.time < nextFireTime)
            return;

        // Decrement ammo
        --ClipSize;

        // Set the next time a shot can be fired.
        nextFireTime = Time.time + FireRate;

        // Calculate a random rotation for the gun spray effect.
        float randomSpread = Random.Range(-SprayAngle, SprayAngle);
        Quaternion sprayRotation = Quaternion.Euler(0, 0, randomSpread);

        // Instantiate the bullet with the calculated spray rotation.
        GameObject projectileGameObject =
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * sprayRotation);

        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Damage = BaseDamage;

        // Play gunshot sound
        SoundEffectManager.Play("Gunshot");
    }

    /// <summary> Coroutine to handle the reloading process. </summary>
    private IEnumerator Reload()
    {
#if UNITY_EDITOR
        int totalAmmoAmountBeforeReload = (int)(ClipSize + ReservedAmmoCount);
#endif

        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload sound
        SoundEffectManager.Play("Reload");

        // Wait.
        yield return new WaitForSeconds(ReloadTime);

        // Move leftover clip ammo back into reserve
        ReservedAmmoCount += ClipSize;
        ClipSize = 0;

        // Replenish the ammo in the clip from reserve
        ClipSize = Mathf.Min(MaxClipSize, ReservedAmmoCount);
        ReservedAmmoCount -= ClipSize;

        isReloading = false;
        Debug.Log("Reload complete!");

#if UNITY_EDITOR
        int totalAmmoAmountAfterReload = (int)(ClipSize + ReservedAmmoCount);
        Debug.Assert(totalAmmoAmountBeforeReload == totalAmmoAmountAfterReload,
            "The total amount of ammo is not conserved!");
#endif
    }
}
