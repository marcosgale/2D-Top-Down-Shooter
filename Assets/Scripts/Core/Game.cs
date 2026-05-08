using UnityEditor;
using UnityEngine;

public class Game
{
    private static GameObject _persistentGameObject;
    public static GameObject PersistentGameObject => _persistentGameObject;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Start()
    {
        _persistentGameObject = new GameObject("PersistentGameObject");
        Object.DontDestroyOnLoad(_persistentGameObject);

        Debug.Log("Initialized persistent game object.");
    }

    public static void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
