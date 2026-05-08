using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public GameObject deathParticlePrefab;
    public GameObject lootPrefab;

    public class B_Enemy
    {
        public string name;
        public int health;
        public float speed;
        public int damage;
        public int ammo;

        public B_Enemy(string name, int health, float speed, int damage, int ammo)
        {
            this.name = name;
            this.health = health;
            this.speed = speed;
            this.damage = damage;
            this.ammo = ammo;
        }
    }

    private B_Enemy enemy; 

    void Start()
    {
        enemy = new B_Enemy("Basic Enemy", 100, 5.5f, 30, 30);

        if (enemy.health <= 0)
        {
            DestroyWithParticles();
        }
    }

    public void TakeDamage(int amount)
    {
        enemy.health -= amount;
        if (enemy.health <= 0)
        {
            DestroyWithParticles();
        }
    }

    private void DestroyWithParticles()
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
            Destroy(gameObject, ps.main.duration);
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (deathParticlePrefab) Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        if (lootPrefab) Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }
}
