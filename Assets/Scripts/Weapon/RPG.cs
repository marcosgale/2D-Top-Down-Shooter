using System.Collections;
using UnityEngine;

public class RPG : Weapon
{
    // --- Public Variables (Configurable in Unity Inspector) ---
    [Header("RPG Settings")]
    [Tooltip("The rocket prefab to be instantiated when shooting.")]
    public GameObject bulletPrefab;
    [Tooltip("The transform where the rocket will be spawned.")]
    public Transform firePoint;

    // --- Private Variables (Internal Game State) ---
    private float nextFireTime = 0f;
    private bool isReloading = false;
    // Track previous frame trigger state for single-shot behavior
    private bool wasTriggerHeldLastFrame = false;

    private void Update()
    {
        // Single-shot: shoot only when trigger goes from not-held -> held (like a real RPG)
        if (IsGunTriggerHeld && !wasTriggerHeldLastFrame)
            Shoot();

        // Check for reload input
        if (ShouldReload && !isReloading && ClipSize < MaxClipSize && ReservedAmmoCount > 0)
            StartCoroutine(Reload());

        wasTriggerHeldLastFrame = IsGunTriggerHeld;
    }

    /// <summary> Fires a single rocket projectile (RPG-style single shot). </summary>
    private void Shoot()
    {
        // Return if the gun is reloading, out of ammo, or still in fire-rate cooldown.
        if (isReloading || ClipSize <= 0 || Time.time < nextFireTime)
            return;

        // Decrement ammo
        --ClipSize;

        // Set the next time a shot can be fired.
        nextFireTime = Time.time + FireRate;

        // Spawn a single rocket (no spread for RPG - fires straight)
        GameObject projectileGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Damage = BaseDamage;
    }

    /// <summary> Coroutine to handle the reloading process. </summary>
    private IEnumerator Reload()
    {
#if UNITY_EDITOR
        int totalAmmoAmountBeforeReload = (int)(ClipSize + ReservedAmmoCount);
#endif

        isReloading = true;
        Debug.Log("Reloading...");

        // Wait.
        yield return new WaitForSeconds(ReloadTime);

        // Remove the ammo from the clip.
        ReservedAmmoCount += ClipSize;
        ClipSize = 0;

        // Replenish the ammo in the clip.
        // If the amount of reserved amount is too low, use that value instead of the max clip size.
        ClipSize = Mathf.Min(MaxClipSize, ReservedAmmoCount);
        ReservedAmmoCount -= ClipSize;

        isReloading = false;
        Debug.Log("Reload complete!");

#if UNITY_EDITOR
        int totalAmmoAmountAfterReload = (int)(ClipSize + ReservedAmmoCount);
        Debug.Assert(totalAmmoAmountBeforeReload == totalAmmoAmountAfterReload, "The total amount of ammo is not conserved!");
#endif
    }

}
