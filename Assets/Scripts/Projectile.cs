using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;

    [SerializeField]
    private float _moveSpeed = 5.0f;

    [SerializeField]
    private float _age = 5.0f;

    [SerializeField]
    private float _damage = 5.0f;
    public float Damage
    {
        get => _damage;
        set => _damage = (value < 0 ? 0 : value);
    }

    [SerializeField, Tooltip("The game object that is spawned when the projectile hits an object.")]
    private GameObject _spawnedObject;

    [HideInInspector]
    public GameObject Instigator;


    private void Update()
    {
        _age -= Time.deltaTime;
        if (_age <= 0.0f)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        _rigidBody.linearVelocity = transform.right * _moveSpeed;
    }

    private void OnDestroy()
    {
        Debug.Log("Projectile destroyed.");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageableObject = collision.gameObject.GetComponentInParent<IDamageable>();
        if (damageableObject != null)
            damageableObject.OnTakeDamage(_damage, collision.GetContact(0).point, Instigator ? Instigator : gameObject);

        // Spawn an object.
        if (_spawnedObject)
        {
            GameObject spawnedObjectInstance = Instantiate(_spawnedObject, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
