using UnityEngine;

public class ActionThrowGrenade : Action
{
    // ActionThrowGrenade.cs (add near the top)
    [SerializeField] private StatComponent _stats;

    public const string STATNAME_GRENADE_COUNT = "Grenade Count";
    public const string STATNAME_MAX_GRENADE_COUNT = "Max Grenade Count";


    [SerializeField]
    private Transform _grenadeSpawnTransform;

    [SerializeField]
    private GameObject _grenadePrefab;

    [SerializeField]
    private float _grenadeLaunchVelocity = 100f;

    [SerializeField, Tooltip("How long before the character can throw another grenade.")]
    private float _maxGrenadeCooldown = 2.0f;

    private float _grenadeCooldown;

    private void Update()
    {
        _grenadeCooldown -= Time.deltaTime;
    }

    protected override bool CanBeExecuted()
    {
        // Must be off cooldown AND have ≥1 grenade
        if (_grenadeCooldown > 0.0f) return false;
        if (_stats == null) return true; // fallback if stats not wired yet
        return _stats.GetValue(STATNAME_GRENADE_COUNT) > 0f;
    }

    protected override void OnExecuted()
    {
        _grenadeCooldown = _maxGrenadeCooldown;

        // Consume 1 grenade if stats are present
        if (_stats != null)
        {
            float cur = _stats.GetValue(STATNAME_GRENADE_COUNT);
            _stats.SetValue(STATNAME_GRENADE_COUNT, Mathf.Max(0f, cur - 1f));
            // Optional UI: GameUIManager.Instance?.SetGrenades((int)_stats.GetValue(STATNAME_GRENADE_COUNT), (int)_stats.GetValue(STATNAME_MAX_GRENADE_COUNT));
        }

        GameObject grenadeGameObject = Instantiate(_grenadePrefab, _grenadeSpawnTransform.position, _grenadeSpawnTransform.rotation);
        Debug.Assert(grenadeGameObject, "grenadeGameObject is null! The grenade prefab may not be set.");

        Grenade grenade = grenadeGameObject.GetComponent<Grenade>();
        Debug.Assert(grenade, "The spawned object doesn't have the Grenade component!");

        Vector3 launchVelocity = _grenadeLaunchVelocity * transform.right;
        grenade.Rigidbody.linearVelocity = launchVelocity;
    }

}
