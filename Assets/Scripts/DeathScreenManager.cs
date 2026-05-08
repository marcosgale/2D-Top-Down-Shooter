using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    private VisualElement root;
    private Button retryButton;
    private Button mainMenuButton;

    private UIDocument uiDocument;

    private void Start()
    {
        PlayerCharacter playerCharacter = PlayerCharacter.Instance;
        playerCharacter.OnHealthReachesZeroEvent += OnHealthReachesZero;

        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Make sure UI is hidden at start
        root.style.display = DisplayStyle.None;

        retryButton = root.Q<Button>("RetryButton");
        Debug.Assert(retryButton != null);

        mainMenuButton = root.Q<Button>("MainMenuButton");
        Debug.Assert(mainMenuButton != null);

        retryButton.clicked += OnRetryButtonClicked;
        mainMenuButton.clicked += OnMainMenuButtonClicked;
    }

    private void OnDestroy()
    {
        if (PlayerCharacter.Instance)
            PlayerCharacter.Instance.OnHealthReachesZeroEvent -= OnHealthReachesZero;
    }

    private void OnHealthReachesZero()
    {
        Debug.Log("TriggerDeath() called — showing UI");
        root.style.display = DisplayStyle.Flex;

        UnityEngine.Cursor.visible = true;
    }

    private void OnRetryButtonClicked()
    {
        // Get the current level.
        Scene sceneToRestart = new Scene();

        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene currentScene = SceneManager.GetSceneAt(i);
            if (currentScene.name == "MainMenu" || currentScene.name == "Player")
                continue;

            sceneToRestart = currentScene;
            break;
        }

        SceneManager.LoadScene(sceneToRestart.name);
    }

    private void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
