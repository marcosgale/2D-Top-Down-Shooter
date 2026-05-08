using UnityEngine;

public class grenadeboxpickup2 : MonoBehaviour
{
    private float grenadesAmount = 4.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter playerCharacter = collision.GetComponentInParent<PlayerCharacter>();
        if (!playerCharacter)
            return;




        if (playerCharacter.IsAmmoFull())
        {
            // If the player's ammo is full, do not pick up the ammo.
            return;
        }

       playerCharacter.AddGrenades(grenadesAmount);
        Destroy(gameObject);
    }
}
