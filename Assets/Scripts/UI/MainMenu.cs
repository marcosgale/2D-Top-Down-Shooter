using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField]
    private string _newGameSceneName;

    private VisualElement _visualElement;
    private Button _newGameButton;
    private Button _exitButton;

    private void Awake()
    {
        Debug.Assert(_uiDocument, "UI Document has not been set!");

        _visualElement = _uiDocument.rootVisualElement;

        _newGameButton = _visualElement.Q<Button>("ButtonNewGame");
        Debug.Assert(_newGameButton != null, "New Game button not found!");
        _newGameButton.clicked += OnNewGameButtonClicked;

        _exitButton = _visualElement.Q<Button>("ButtonExit");
        Debug.Assert(_exitButton != null, "Exit button not found!");
        _exitButton.clicked += OnExitButtonClicked;
    }

    private void OnNewGameButtonClicked()
    {
        SceneManager.LoadScene(_newGameSceneName);
    }

    private void OnExitButtonClicked()
    {
        Game.Exit();
    }
}
