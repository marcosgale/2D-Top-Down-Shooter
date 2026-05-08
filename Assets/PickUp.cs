using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private float ammoAmount = 100.0f;


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

        playerCharacter.AddAmmo(ammoAmount);
        Destroy(gameObject);    
    }
}
