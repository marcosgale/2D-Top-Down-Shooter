using UnityEngine;

public class TestCharacter : Character
{
    private static int _instanceCount = 0;
    public static int InstanceCount => _instanceCount;

    [Header("Debug Info (Read-Only)")]
    [SerializeField, Tooltip("Current number of TestCharacter instances")]
    private int _debugInstanceCount = 0;

    private void OnEnable()
    {
        _instanceCount++;
        _debugInstanceCount = _instanceCount;
    }

    private void OnDisable()
    {
        _instanceCount--;
        _debugInstanceCount = _instanceCount;
    }

    private void Update()
    {
        // Update debug counter every frame so it's visible in inspector
        _debugInstanceCount = _instanceCount;
    }

    public override void OnHealthReachesZero()
    {
        //SoundEffectManager.Play("EnemyDeath");
        AudioSourcePool.Instance.PlayOneShot(transform.position, DeathClipGroup.GetRandomAudioClip());
        Destroy(gameObject);
    }
}
