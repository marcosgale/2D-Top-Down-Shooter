using UnityEngine;

/// <summary>
/// Manages the progression of scenes/rooms in order.
/// Place this in your first scene or make it persist across scenes.
/// </summary>
public class SceneProgressionManager : MonoBehaviour
{
    private static SceneProgressionManager _instance;
    public static SceneProgressionManager Instance => _instance;

    [SerializeField, Tooltip("List of scene names in progression order")]
    private string[] _sceneProgression = new string[]
    {
        "TestScene1",
        "TestScene2",
        "TestScene3"
    };

    private int _currentSceneIndex = 0;

    public string CurrentSceneName => _currentSceneIndex < _sceneProgression.Length 
        ? _sceneProgression[_currentSceneIndex] 
        : "";

    public string NextSceneName => (_currentSceneIndex + 1) < _sceneProgression.Length 
        ? _sceneProgression[_currentSceneIndex + 1] 
        : "";

    public bool IsLastScene => (_currentSceneIndex + 1) >= _sceneProgression.Length;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Sync the current scene index with the actual loaded scene
        UpdateCurrentSceneIndex();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Update the progression index whenever a new scene loads
        UpdateCurrentSceneIndex();
    }

    private void UpdateCurrentSceneIndex()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        for (int i = 0; i < _sceneProgression.Length; i++)
        {
            if (_sceneProgression[i] == currentScene)
            {
                _currentSceneIndex = i;
                Debug.Log($"SceneProgressionManager synced to scene: {currentScene} (index {i})");
                return;
            }
        }
    }

    /// <summary>
    /// Get the next scene in progression.
    /// </summary>
    public string GetNextScene()
    {
        if (IsLastScene)
        {
            Debug.LogWarning("Already at the last scene!");
            return CurrentSceneName;
        }

        _currentSceneIndex++;
        Debug.Log($"Progressing to scene {_currentSceneIndex}: {CurrentSceneName}");
        return CurrentSceneName;
    }

    /// <summary>
    /// Set the current scene by name (useful for loading saves or testing).
    /// </summary>
    public void SetCurrentScene(string sceneName)
    {
        for (int i = 0; i < _sceneProgression.Length; i++)
        {
            if (_sceneProgression[i] == sceneName)
            {
                _currentSceneIndex = i;
                Debug.Log($"Scene progression set to index {i}: {sceneName}");
                return;
            }
        }
        Debug.LogWarning($"Scene '{sceneName}' not found in progression list!");
    }

    /// <summary>
    /// Reset progression to the first scene.
    /// </summary>
    public void ResetProgression()
    {
        _currentSceneIndex = 0;
        Debug.Log("Scene progression reset to start.");
    }

    /// <summary>
    /// Get scene at a specific index.
    /// </summary>
    public string GetSceneAt(int index)
    {
        if (index >= 0 && index < _sceneProgression.Length)
            return _sceneProgression[index];
        
        Debug.LogWarning($"Scene index {index} out of range!");
        return "";
    }
}
