using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField]
    private string mainMenuScene = "MainMenu";

    [SerializeField]
    private string _inGameActionMap = "Player";

    [SerializeField]
    private string _uiActionMap = "UI";

    private VisualElement _visualElement;
    private Button _buttonResume;
    private Button _buttonMainMenu;

    private void OnValidate()
    {
        if (!_uiDocument)
            _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        Time.timeScale = 0.0f;

        if (PlayerInput.all.Count > 0)
            PlayerInput.all[0].SwitchCurrentActionMap(_uiActionMap);

        _visualElement = _uiDocument.rootVisualElement;

        _buttonResume = _visualElement.Q<Button>("ButtonResume");
        _buttonResume.clicked += OnButtonResumeClicked;

        _buttonMainMenu = _visualElement.Q<Button>("ButtonMainMenu");
        _buttonMainMenu.clicked += OnButtonMainMenuClicked;
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;

        if (PlayerInput.all.Count > 0)
            PlayerInput.all[0].SwitchCurrentActionMap(_inGameActionMap);

        if (_buttonResume != null)
            _buttonResume.clicked -= OnButtonResumeClicked;

        if (_buttonMainMenu != null)
            _buttonMainMenu.clicked -= OnButtonMainMenuClicked;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
    }

    private void OnButtonResumeClicked()
    {
        Resume();
    }

    private void OnButtonMainMenuClicked()
    {
        SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
    }
}
