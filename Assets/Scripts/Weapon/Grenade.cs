using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [SerializeField]
    private float _durationBeforeExplosion = 3.0f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private void Start()
    {
        StartCoroutine(WaitBeforeExplosion());
    }

    public void OnTakeDamage(float amount, GameObject instigator)
    {
        Explode();
    }

    private IEnumerator WaitBeforeExplosion()
    {
        yield return new WaitForSeconds(_durationBeforeExplosion);
        Explode();
    }

    public void Explode()
    {
        // ?? Play explosion sound
        //SoundEffectManager.Play("GrenadeExplosion");

        Instantiate(_explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
