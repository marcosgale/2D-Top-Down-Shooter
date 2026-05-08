using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SplashMessageUIToolkit : MonoBehaviour
{
    private static SplashMessageUIToolkit _instance;
    public static SplashMessageUIToolkit Instance => _instance;

    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField]
    private float _displayDuration = 2.0f;

    [SerializeField]
    private float _fadeInDuration = 0.3f;

    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    private VisualElement _container;
    private Label _messageLabel;
    private Coroutine _currentMessageCoroutine;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (!_uiDocument)
            _uiDocument = GetComponent<UIDocument>();

        if (_uiDocument != null)
        {
            var root = _uiDocument.rootVisualElement;
            _container = root.Q<VisualElement>("SplashMessageContainer");
            _messageLabel = root.Q<Label>("SplashMessageLabel");

            // Start invisible
            if (_container != null)
            {
                _container.style.opacity = 0;
                _container.style.display = DisplayStyle.None;
            }
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    /// <summary>
    /// Display a splash message on screen
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">How long to display it (optional, uses default if not specified)</param>
    public void ShowMessage(string message, float? duration = null)
    {
        if (_currentMessageCoroutine != null)
            StopCoroutine(_currentMessageCoroutine);

        _currentMessageCoroutine = StartCoroutine(ShowMessageCoroutine(message, duration ?? _displayDuration));
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        if (_messageLabel == null || _container == null)
            yield break;

        // Set the message and show container
        _messageLabel.text = message;
        _container.style.display = DisplayStyle.Flex;

        // Fade in
        float elapsed = 0;
        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / _fadeInDuration);
            _container.style.opacity = alpha;
            yield return null;
        }
        _container.style.opacity = 1;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsed = 0;
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / _fadeOutDuration);
            _container.style.opacity = alpha;
            yield return null;
        }
        _container.style.opacity = 0;
        _container.style.display = DisplayStyle.None;
        _messageLabel.text = "";

        _currentMessageCoroutine = null;
    }
}
