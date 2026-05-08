using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private InGameUI _instance;
    public InGameUI Instance => _instance;

    [SerializeField]
    private BlackScreen _blackScreen;
    public BlackScreen BlackScreen => _blackScreen;

    [SerializeField]
    private PauseMenu _pauseMenu;
    public PauseMenu PauseMenu => _pauseMenu;

    [SerializeField]
    private SplashMessage _splashMessage;
    public SplashMessage SplashMessage => _splashMessage;

    private void OnValidate()
    {
        if (!_blackScreen)
            _blackScreen = GetComponentInChildren<BlackScreen>(true);
        if (!_pauseMenu)
            _pauseMenu = GetComponentInChildren<PauseMenu>(true);
        if (!_splashMessage)
            _splashMessage = GetComponentInChildren<SplashMessage>(true);
    }

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
}
