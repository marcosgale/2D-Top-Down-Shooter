using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    public delegate void OnTakeDamageDelegate(float amount, Vector2 impactPoint, GameObject instigator);
    public OnTakeDamageDelegate OnTakeDamageEvent;

    public delegate void OnHealthReachesZeroDelegate();
    public OnHealthReachesZeroDelegate OnHealthReachesZeroEvent;

    [Header("Movement")]

    [SerializeField]
    private CharacterMovement _characterMovement;
    public CharacterMovement CharacterMovement => _characterMovement;

    [Header("Stats")]

    [SerializeField]
    private StatComponent _statComponent;
    public StatComponent StatComponent => _statComponent;

    public float Health => StatComponent.GetValue("Health");
    public float MaxHealth => StatComponent.GetValue("Max Health");
    public float GrenadeCount => StatComponent.GetValue("Grenade Count");

    [SerializeField, Tooltip("When enabled, this character never takes damage.")]
    private bool _godMode;

    [Header("Actions")]

    [SerializeField]
    private ActionComponent _actionComponent;
    public ActionComponent ActionComponent => _actionComponent;

    [Header("Sprites")]

    [SerializeField]
    private bool _shouldSpawnBloodSplash = true;

    [SerializeField]
    private GameObject _bloodSplashPrefab;

    [Header("Sound")]

    [SerializeField]
    private AudioClipGroup _deathClipGroup;
    public AudioClipGroup DeathClipGroup => _deathClipGroup;

    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

    protected virtual void OnValidate()
    {
        if (!_characterMovement)
            _characterMovement = GetComponent<CharacterMovement>();

        if (!_statComponent)
            _statComponent = GetComponent<StatComponent>();

        if (!_actionComponent)
            _actionComponent = GetComponent<ActionComponent>();
    }

    public void Heal(float amount)
    {
        float currentHealth = StatComponent.GetValue("Health");

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0.0f, StatComponent.GetValue("Max Health"));

        StatComponent.SetValue("Health", currentHealth);
    }

    public void OnTakeDamage(float amount, Vector2 impactPoint, GameObject instigator)
    {
        if (_godMode)
            return;

        // Reduce health.
        float currentHealth = _statComponent.GetValue("Health");
        currentHealth -= amount;
        _statComponent.SetValue("Health", currentHealth);
        OnTakeDamageEvent?.Invoke(amount, impactPoint, instigator);

        // If the heal reaches 0, trigger an event.
        if (currentHealth <= 0 && _isAlive)
        {
            _isAlive = false;
            OnHealthReachesZero();
            OnHealthReachesZeroEvent?.Invoke();
        }

        // Spawn blood splash.
        if (_shouldSpawnBloodSplash)
            SpawnBloodSplash(transform.position);
    }

    public virtual void OnHealthReachesZero()
    {

    }

    public void SpawnBloodSplash(Vector2 impactPoint)
    {
        GameObject bloodSplashGameObject = Instantiate(_bloodSplashPrefab, impactPoint, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
    }
}
