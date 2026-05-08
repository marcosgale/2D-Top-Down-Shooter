using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public GameObject deathParticlePrefab;
    public GameObject lootPrefab;
    private void OnDestroy()
    {
        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        Instantiate(lootPrefab, transform.position, Quaternion.identity);
    }

    public class K_Enemy
    {
        public string Kenemy;
        public int health;
        public float speed;
        public int damage;

        public K_Enemy(string name, int health, float speed, int damage)
        {
            this.Kenemy = name;
            this.health = health;
            this.speed = speed;
            this.damage = damage;
        }
    }


    void Start()
    {
        K_Enemy Kenemy = new K_Enemy("Knife Enemy", 100, 5.5f, 55);
        if (Kenemy.health <= 0)
        {
            Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
        }
    }

    void Update()
    {
        
    }
}
