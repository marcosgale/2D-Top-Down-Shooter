using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPosition;

    public void Fire()
    {
        GameObject projectile = Instantiate(
            _projectilePrefab,
            _projectileSpawnPosition.position,
            _projectileSpawnPosition.rotation
        );

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
            projectileScript.Instigator = gameObject; // Assign the shooter (e.g., player or turret)
    }
}
