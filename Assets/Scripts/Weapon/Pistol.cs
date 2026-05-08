using System.Collections;
using UnityEngine;

public class Pistol : Weapon
{
    // --- Public Variables (Configurable in Unity Inspector) ---
    [Header("Gun Settings")]
    [Tooltip("The bullet prefab to be instantiated when shooting.")]
    public GameObject bulletPrefab;
    [Tooltip("The transform where bullets will be spawned.")]
    public Transform firePoint;

    // --- Private Variables (Internal Game State) ---
    private float nextFireTime = 0f;
    private bool isReloading = false;
    // track previous frame trigger state to detect rising edge (one shot per press)
    private bool wasTriggerHeldLastFrame = false;

    private void Update()
    {
        // Semi-auto: shoot only when trigger goes from not-held -> held
        if (IsGunTriggerHeld && !wasTriggerHeldLastFrame)
            Shoot();

        // Check for reload input
        if (ShouldReload && !isReloading && ClipSize < MaxClipSize && ReservedAmmoCount > 0)
            StartCoroutine(Reload());

        wasTriggerHeldLastFrame = IsGunTriggerHeld;
    }

    // Semi-automatic shooting - fires as fast as you can pull the trigger (no fire rate limit)
    private void Shoot()
    {
        // Return if the gun is reloading or out of ammo.
        if (isReloading || ClipSize <= 0)
            return;

        // Decrement ammo
        --ClipSize;

        // Instantiate the bullet with no spray rotation.
        GameObject projectileGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Damage = BaseDamage;

        // Play gunshot sound
        AudioSourcePool.Instance.PlayOneShot(transform.position, FireClipGroup);
    }

    /// <summary> Coroutine to handle the reloading process. </summary>
    private IEnumerator Reload()
    {
#if UNITY_EDITOR
        int totalAmmoAmountBeforeReload = (int)(ClipSize + ReservedAmmoCount);
#endif

        isReloading = true;
        Debug.Log("Reloading...");

        AudioSourcePool.Instance.PlayOneShot(transform.position, ReloadClipGroup);

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
        //nice
#if UNITY_EDITOR
        int totalAmmoAmountAfterReload = (int)(ClipSize + ReservedAmmoCount);
        Debug.Assert(totalAmmoAmountBeforeReload == totalAmmoAmountAfterReload, "The total amount of ammo is not conserved!");
#endif
    }
}
