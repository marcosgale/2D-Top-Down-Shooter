using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void OnFadeInStartedDelegate();
public delegate void OnFadeInFinishedDelegate();
public delegate void OnFadeOutStartedDelegate();
public delegate void OnFadeOutFinishedDelegate();

public struct FadeInProperties
{
    public float FadeInDuration;
    public OnFadeInStartedDelegate OnFadeInStarted;
    public OnFadeInFinishedDelegate OnFadeInFinished;
}

public struct FadeOutProperties
{
    public float FadeOutDuration;
    public OnFadeOutStartedDelegate OnFadeOutStarted;
    public OnFadeOutFinishedDelegate OnFadeOutFinished;
}

public class BlackScreen : MonoBehaviour
{
    private static BlackScreen _instance;
    public static BlackScreen Instance => _instance;

    [SerializeField]
    private UIDocument _uiDocument;

    private VisualElement _visualElement;
    private VisualElement _background;

    private void OnValidate()
    {
        if (!_uiDocument)
            _uiDocument = GetComponent<UIDocument>();
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(_instance);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        _visualElement = _uiDocument.rootVisualElement;
        _background = _visualElement.Q("Background");
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public IEnumerator FadeInCoroutine(FadeInProperties fadeInProperties)
    {
        if (fadeInProperties.OnFadeInStarted != null)
            fadeInProperties.OnFadeInStarted.Invoke();

        while (_background.style.backgroundColor.value.a < 1.0f)
        {
            Color currentColor = _background.style.backgroundColor.value;
            currentColor.a += Time.deltaTime / fadeInProperties.FadeInDuration;

            _background.style.backgroundColor = currentColor;

            yield return null;
        }

        if (fadeInProperties.OnFadeInFinished != null)
            fadeInProperties.OnFadeInFinished.Invoke();
    }

    public IEnumerator FadeOutCoroutine(FadeOutProperties fadeOutProperties)
    {
        if (fadeOutProperties.OnFadeOutStarted != null)
            fadeOutProperties.OnFadeOutFinished.Invoke();

        while (_background.style.backgroundColor.value.a > 0.0f)
        {
            Color currentColor = _background.style.backgroundColor.value;
            currentColor.a -= Time.deltaTime / fadeOutProperties.FadeOutDuration;

            _background.style.backgroundColor = currentColor;

            yield return null;
        }

        if (fadeOutProperties.OnFadeOutStarted != null)
            fadeOutProperties.OnFadeOutFinished.Invoke();
    }
}
