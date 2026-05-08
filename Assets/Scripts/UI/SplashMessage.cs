using System.Collections;
using UnityEngine;
using TMPro;

public class SplashMessage : MonoBehaviour
{
    private static SplashMessage _instance;
    public static SplashMessage Instance => _instance;

    [SerializeField]
    private TextMeshProUGUI _messageText;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _displayDuration = 2.0f;

    [SerializeField]
    private float _fadeInDuration = 0.3f;

    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    private Coroutine _currentMessageCoroutine;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (!_canvasGroup)
            _canvasGroup = GetComponent<CanvasGroup>();

        if (!_messageText)
            _messageText = GetComponentInChildren<TextMeshProUGUI>();

        // Start invisible
        if (_canvasGroup)
            _canvasGroup.alpha = 0;
        
        if (_messageText)
            _messageText.text = "";
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
        if (!_messageText || !_canvasGroup)
            yield break;

        // Set the message
        _messageText.text = message;

        // Fade in
        float elapsed = 0;
        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / _fadeInDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1;

        // Hold
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsed = 0;
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / _fadeOutDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0;
        _messageText.text = "";

        _currentMessageCoroutine = null;
    }
}
