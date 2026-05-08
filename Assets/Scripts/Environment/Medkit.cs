using UnityEngine;

public class Medkit : MonoBehaviour
{
    [SerializeField]
    private float _healAmount = 20.0f;

    [SerializeField]
    private AudioClipGroup _healClipGroup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter playerCharacter = collision.GetComponentInParent<PlayerCharacter>();
        if (!playerCharacter)
            return;

        playerCharacter.Heal(_healAmount);

        //SoundEffectManager.Play("Heal");
        AudioSourcePool.Instance.PlayOneShot(transform.position, _healClipGroup.GetRandomAudioClip());

        Destroy(gameObject);
    }
}
