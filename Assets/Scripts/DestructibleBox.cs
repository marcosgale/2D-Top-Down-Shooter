using UnityEngine;

public class DestructibleBox : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _health = 50.0f;

    [SerializeField]
    private GameObject _dropItemPrefab;

    [SerializeField, Range(0f, 1f)]
    private float _dropChance = 0.25f; // 25% chance

    public void OnTakeDamage(float damage, Vector2 impactPoint, GameObject instigator)
    {
        _health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {_health}");

        if (_health <= 0)
        {
            TryDropItem();
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} was destroyed.");
        }
    }

    private void TryDropItem()
    {
        if (_dropItemPrefab != null && Random.value <= _dropChance)
        {
            Instantiate(_dropItemPrefab, transform.position, Quaternion.identity);
            Debug.Log($"Dropped item: {_dropItemPrefab.name}");
        }
        else
        {
            Debug.Log("No item dropped.");
        }
    }
}
