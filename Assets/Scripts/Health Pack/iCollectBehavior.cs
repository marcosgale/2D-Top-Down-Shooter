using UnityEngine;

public interface iCollectBehavior
{
    void OnCollected(GameObject player);
    void OnCollected(object gameObject);
}
