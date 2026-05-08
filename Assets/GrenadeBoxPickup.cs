using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GrenadeBoxPickup : MonoBehaviour
{
    [Header("Stats Keys (match your StatComponent)")]
    public string STATNAME_GRENADE_COUNT = "Grenade Count";
    public string STATNAME_MAX_GRENADE_COUNT = "Max Grenade Count";

    [Header("Pickup Amount")]
    public float grenadesToGive = 1f;

    [Header("Destroy after pickup")]
    public bool destroyOnPickup = true;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true; // pickups shouldn't block
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Get StatComponent from player (or child)
        var stats = other.GetComponent<StatComponent>() ?? other.GetComponentInChildren<StatComponent>();
        if (stats == null)
        {
            Debug.LogWarning("[GrenadeBoxPickup] No StatComponent found on Player.");
            return;
        }

        float cur = stats.GetValue(STATNAME_GRENADE_COUNT);
        float max = stats.GetValue(STATNAME_MAX_GRENADE_COUNT);

        if (cur >= max)
        {
            Debug.Log("[GrenadeBoxPickup] Grenades already at max.");
            return;
        }

        float newVal = Mathf.Min(cur + grenadesToGive, max);
        stats.SetValue(STATNAME_GRENADE_COUNT, newVal);

        Debug.Log($"[GrenadeBoxPickup] +{newVal - cur} grenade(s). Now {newVal}/{max}.");

        // Optional UI update:
        // GameUIManager.Instance?.SetGrenades((int)newVal, (int)max);

        if (destroyOnPickup) Destroy(gameObject);
    }
}
