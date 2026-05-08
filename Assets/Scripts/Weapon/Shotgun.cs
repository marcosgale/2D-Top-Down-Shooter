using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
{
    // --- Public Variables (Configurable in Unity Inspector) ---
    [Header("Gun Settings")]
    [Tooltip("The bullet prefab to be instantiated when shooting.")]
    public GameObject bulletPrefab;
    [Tooltip("The transform where bullets will be spawned.")]
    public Transform firePoint;

    public int PelletsPerShot => (int)StatComponent.GetValue("Pellets per Shot");

    // --- Private Variables (Internal Game State) ---
    private float nextFireTime = 0f;
    private bool isReloading = false;

    private void Update()
    {
        // Check for player input to shoot
        if (IsGunTriggerHeld && Time.time >= nextFireTime)
            Shoot();

        // Check for reload input
        if (ShouldReload && !isReloading && ClipSize < MaxClipSize && ReservedAmmoCount > 0)
            StartCoroutine(Reload());
    }

    /// <summary> Instantiates a bullet, handles the spray, and manages the fire rate. </summary>
    private void Shoot()
    {
        // Return if the gun is reloading or out of ammo.
        if (isReloading || ClipSize <= 0)
            return;

        // Decrement ammo
        --ClipSize;

        // Set the next time a shot can be fired.
        nextFireTime = Time.time + FireRate;

        // Spawn the pellets.
        for (int i = 0; i < PelletsPerShot; ++i)
        {
            float randomSpread = Random.Range(-SprayAngle, SprayAngle);
            Quaternion sprayRotation = Quaternion.Euler(0, 0, randomSpread);

            GameObject projectileGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * sprayRotation);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Damage = BaseDamage;
        }

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
