using Unity.VisualScripting;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField]
    private AISight _aiSight;

    public GameObject bulletPrefab;
    public Transform firePoint;

    [SerializeField]
    private float _waitTime = 1.4f;

    private float _cooldown;

    private void OnValidate()
    {
        if (!_aiSight)
            _aiSight = GetComponentInChildren<AISight>();
    }

    private void Start()
    {
        _cooldown = _waitTime;
    }

    private void Update()
    {
        if (CanSeeTarget() && IsShootingAngleGood())
        {
            if (_cooldown <= 0.0f)
            {
                Shoot();
                _cooldown = 1.0f;
            }
        }

        _cooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        float randomSpread = Random.Range(-5f, 5f);
        Quaternion sprayRotation = Quaternion.Euler(0, 0, randomSpread);
        // Instantiate the bullet with the calculated spray rotation.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * sprayRotation);
    }

    private bool CanSeeTarget()
    {
        if (!PlayerCharacter.Instance || !PlayerCharacter.Instance.IsAlive || !_aiSight.CanSeeObject(PlayerCharacter.Instance.gameObject))
            return false;

        return true;
    }

    private bool IsShootingAngleGood()
    {
        if (!PlayerCharacter.Instance)
            return false;

        Vector2 directionToTarget = PlayerCharacter.Instance.transform.position - transform.position;
        directionToTarget = directionToTarget.normalized;

        Vector2 forwardDirection = transform.right;

        float dotProduct = Vector2.Dot(directionToTarget, forwardDirection);
        if (dotProduct < 0.97f)
            return false;

        return true;
    }
}
