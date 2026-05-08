using UnityEngine;

public class Collectable : MonoBehaviour
{
    private iCollectBehavior _collectableBahaviour;

    private void Awake()
    {
        _collectableBahaviour = GetComponent<iCollectBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerMovement>();


        if (player != null )
        {
            _collectableBahaviour.OnCollected(player.gameObject);
            Destroy(gameObject);
        }
    }

    private class PlayerMovement
    {
        public GameObject gameObject { get; internal set; }
    }
}
