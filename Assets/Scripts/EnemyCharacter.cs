using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    [SerializeField]
    private StatComponent _statsComponent;

    private bool _isRegistered = false;

    private void OnValidate()
    {
        if (!_statsComponent)
            _statsComponent = GetComponent<StatComponent>();
    }

    private void OnEnable()
    {
        // Register this enemy with the room manager
        if (RoomManager.Instance != null && !_isRegistered)
        {
            RoomManager.Instance.RegisterEnemy(gameObject);
            _isRegistered = true;
        }
    }

    private void OnDisable()
    {
        // Unregister this enemy when disabled/destroyed
        if (RoomManager.Instance != null && _isRegistered)
        {
            RoomManager.Instance.UnregisterEnemy(gameObject);
            _isRegistered = false;
        }
    }

    private void OnHealthReachesZero()
    {
        // Unregister from room manager BEFORE destroying
        if (RoomManager.Instance != null && _isRegistered)
        {
            RoomManager.Instance.UnregisterEnemy(gameObject);
            _isRegistered = false;
            Debug.Log($"EnemyCharacter {gameObject.name} died and unregistered from room manager");
        }

        Destroy(gameObject);
    }

    public void OnTakeDamage(float amount, GameObject instigator)
    {
        float currentHealth = _statsComponent.GetValue("Health");
        currentHealth -= amount;
        _statsComponent.SetValue("Health", currentHealth);

        if (currentHealth <= 0)
        {
            OnHealthReachesZero();
        }
    }
}
