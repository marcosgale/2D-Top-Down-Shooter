//Gun.cs
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all the core gun mechanics for a top-down shooter, including
/// shooting, reloading, and a gun spray effect.
/// </summary>
public class Rifle : Weapon
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

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    private void Update()
    {
        // Check for player input to shoot
        if (IsGunTriggerHeld && Time.time >= nextFireTime)
            Shoot();

        // Check for reload input
        if (ShouldReload && !isReloading && ClipSize < MaxClipSize && ReservedAmmoCount > 0)
            StartCoroutine(Reload());
    }

    public void AddReservedAmmo(float amount)
    {
        if (StatComponent == null) return;

        // Clamp reserved ammo to MaxAmmoCount
        ReservedAmmoCount = Mathf.Clamp(ReservedAmmoCount + amount, 0f, MaxAmmoCount);
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

        // Calculate a random rotation for the gun spray effect.
        float randomSpread = Random.Range(-SprayAngle, SprayAngle);
        Quaternion sprayRotation = Quaternion.Euler(0, 0, randomSpread);

        // Instantiate the bullet with the calculated spray rotation.
        GameObject projectileGameObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * sprayRotation);
        Projectile projectile = projectileGameObject.GetComponent<Projectile>();
        projectile.Damage = BaseDamage;

        // Play gunshot sound
        //SoundEffectManager.Play("Gunshot");
        AudioSourcePool.Instance.PlayOneShot(transform.position, FireClipGroup.GetRandomAudioClip());
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
        //SoundEffectManager.Play("Reload");
        AudioSourcePool.Instance.PlayOneShot(transform.position, ReloadClipGroup.GetRandomAudioClip());

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
