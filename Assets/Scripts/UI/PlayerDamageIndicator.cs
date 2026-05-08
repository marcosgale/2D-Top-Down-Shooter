using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDamageIndicator : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField]
    private float _fadeOutDuration = 0.2f;

    private VisualElement _visualElement;
    private VisualElement _background;

    private Coroutine _fadeCoroutine = null;

    private void OnValidate()
    {
        if (!_uiDocument)
            _uiDocument = GetComponent<UIDocument>();
    }

    private void Start()
    {
        _visualElement = _uiDocument.rootVisualElement;
        _background = _visualElement.Q("Background");

        Debug.Assert(PlayerCharacter.Instance, "Player character not found!", this);
        PlayerCharacter.Instance.OnTakeDamageEvent += OnTakeDamage;
    }

    public void OnDestroy()
    {
        if (PlayerCharacter.Instance)
            PlayerCharacter.Instance.OnTakeDamageEvent -= OnTakeDamage;
    }

    private void OnTakeDamage(float amount, Vector2 impactPoint, GameObject instigator)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeScreenToRed());
    }

    private IEnumerator FadeScreenToRed()
    {
        // Make the screen red.
        Color currentColor = new Color(0.4f, 0.0f, 0.0f, 1.0f);
        _background.style.backgroundColor = currentColor;
        yield return null;

        // Fade out the red color.
        while (_background.style.backgroundColor.value.a > 0.0f)
        {
            currentColor = _background.style.backgroundColor.value;
            currentColor.a -= Time.deltaTime / _fadeOutDuration;

            _background.style.backgroundColor = currentColor;

            yield return null;
        }

        _fadeCoroutine = null;
        yield return null;
    }
}
