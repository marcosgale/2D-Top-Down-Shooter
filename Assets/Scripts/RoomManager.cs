using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a room by tracking all enemies and determining when the room is cleared.
/// </summary>
public class RoomManager : MonoBehaviour
{
    private static RoomManager _instance;
    public static RoomManager Instance => _instance;

    private HashSet<GameObject> _activeEnemies = new HashSet<GameObject>();
    
    [SerializeField, Tooltip("If true, room starts locked until all enemies are cleared")]
    private bool _requireClearToProgress = true;
    public bool RequireClearToProgress => _requireClearToProgress;

    [Header("Debug Info (Read-Only)")]
    [SerializeField, Tooltip("Current number of enemies in the room")]
    private int _debugEnemyCount = 0;

    public bool IsRoomCleared => _activeEnemies.Count == 0;
    public int EnemyCount => _activeEnemies.Count;

    public delegate void RoomClearedDelegate();
    public event RoomClearedDelegate OnRoomCleared;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Multiple RoomManagers detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Debug.Log("RoomManager initialized.");
    }

    private void Start()
    {
        // Count all enemies already in the scene at start
        RegisterExistingEnemies();
        _debugEnemyCount = _activeEnemies.Count;
        Debug.Log($"Room started with {_activeEnemies.Count} enemies.");
    }

    private void Update()
    {
        // Update debug counter every frame so it's visible in inspector
        _debugEnemyCount = _activeEnemies.Count;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    /// <summary>
    /// Find and register all existing enemies in the scene.
    /// Called at Start to catch enemies that were already in the scene.
    /// </summary>
    private void RegisterExistingEnemies()
    {
        // Find all TestCharacter enemies
        TestCharacter[] testCharacters = FindObjectsByType<TestCharacter>(FindObjectsSortMode.None);
        foreach (var enemy in testCharacters)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                RegisterEnemy(enemy.gameObject);
            }
        }

        // Find all EnemyCharacter enemies
        EnemyCharacter[] enemyCharacters = FindObjectsByType<EnemyCharacter>(FindObjectsSortMode.None);
        foreach (var enemy in enemyCharacters)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                RegisterEnemy(enemy.gameObject);
            }
        }
    }

    /// <summary>
    /// Register an enemy with the room manager.
    /// </summary>
    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("Attempted to register null enemy!");
            return;
        }

        bool wasAdded = _activeEnemies.Add(enemy);
        if (wasAdded)
        {
            _debugEnemyCount = _activeEnemies.Count;
            Debug.Log($"<color=green>✓ Enemy REGISTERED: {enemy.name} | Total: {_activeEnemies.Count}</color>");
        }
        else
        {
            Debug.LogWarning($"Enemy {enemy.name} was already registered!");
        }
    }

    /// <summary>
    /// Unregister an enemy (called when enemy dies).
    /// </summary>
    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("Attempted to unregister null enemy!");
            return;
        }

        bool wasRemoved = _activeEnemies.Remove(enemy);
        if (wasRemoved)
        {
            _debugEnemyCount = _activeEnemies.Count;
            Debug.Log($"<color=red>✗ Enemy UNREGISTERED: {enemy.name} | Remaining: {_activeEnemies.Count}</color>");

            if (IsRoomCleared)
            {
                Debug.Log("<color=yellow>=== ROOM CLEARED! ===</color>");
                OnRoomCleared?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to unregister {enemy.name} but it wasn't in the list! Current count: {_activeEnemies.Count}");
        }
    }

    /// <summary>
    /// Manually clear all enemy registrations (useful for testing or room resets).
    /// </summary>
    public void ClearAllEnemies()
    {
        _activeEnemies.Clear();
        Debug.Log("All enemies cleared from room manager.");
    }
}
