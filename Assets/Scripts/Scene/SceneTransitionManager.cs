using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Handles transition from one scene to another. Should be put in the Player scene. </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance => _instance;

    [SerializeField]
    private string _mainMenuSceneName = "MainMenu";

    [SerializeField]
    private string _playerSceneName = "Player";

    private Coroutine _sceneTransitionCoroutine = null;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public void SwitchToScene(string sceneName)
    {
        if (_sceneTransitionCoroutine != null)
            StopCoroutine(_sceneTransitionCoroutine);
        _sceneTransitionCoroutine = StartCoroutine(SceneTransitionCoroutine(sceneName));
    }

    private IEnumerator SceneTransitionCoroutine(string sceneName)
    {
        // Fade in the blackscreen.
        FadeInProperties blackSreenFadeInProperties = new FadeInProperties()
        {
            FadeInDuration = 1.0f
        };
        yield return BlackScreen.Instance.FadeInCoroutine(blackSreenFadeInProperties);


        // Tell the scene manager to unload all scenes, except the Player scene.
        List<AsyncOperation> unloadOperations = new List<AsyncOperation>();
        int loadedSceneCount = SceneManager.loadedSceneCount;
        for (int i = 0; i < loadedSceneCount; ++i)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == gameObject.scene.name)
                continue;

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadedScene);
            unloadOperations.Add(unloadOperation);
        }

        // Wait until all the scenes are unloaded.
        foreach (AsyncOperation unloadOperation in unloadOperations)
        {
            while (!unloadOperation.isDone)
                yield return null;
        }

        // Load the next scene. If the next scene is the main menu, only the main menu is loaded.
        LoadSceneMode loadSceneMode = (sceneName != _mainMenuSceneName ? LoadSceneMode.Additive : LoadSceneMode.Single);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        while (!loadOperation.isDone)
            yield return null;

        _sceneTransitionCoroutine = null;

        // Fade out the black screen.
        FadeOutProperties blackSreenFadeOutProperties = new FadeOutProperties()
        {
            FadeOutDuration = 1.0f
        };
        yield return BlackScreen.Instance.FadeOutCoroutine(blackSreenFadeOutProperties);
    }
}
