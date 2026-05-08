using UnityEngine;

public class Grenadescript : MonoBehaviour
{

    public GameObject deathParticlePrefab;
    public GameObject lootPrefab;
    private void OnDestroy()
    {
        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }
    public class G_Enemy
    {
        public string Benemy;
        public int health;
        public float speed;
        public int damage;
        public int ammo;

        public G_Enemy(string name, int health, float speed, int damage, int ammo)
        {
            this.Benemy = name;
            this.health = health;
            this.speed = speed;
            this.damage = damage;
            this.ammo = ammo;
        }
    }

    void Start()
    {
        G_Enemy Genemy = new G_Enemy("Grenade Enemy", 100, 5.5f, 50, 8);
        if (Genemy.health <= 0)
        {
            Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
        }
    }

    void Update()
    {

    }
}

