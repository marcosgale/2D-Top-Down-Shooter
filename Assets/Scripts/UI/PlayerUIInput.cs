using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIInput : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _inputPause;

    [SerializeField]
    private PauseMenu _pauseMenu;

    private void OnEnable()
    {
        Debug.Assert(_inputPause.action != null, "_inputPause.action is null! It's possible that PlayerInput component doesn't exist in the scene.");
        _inputPause.action.performed += OnPauseButtonPressed;
    }

    private void OnDisable()
    {
        if (_inputPause.action != null)
            _inputPause.action.performed -= OnPauseButtonPressed;
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        _pauseMenu.gameObject.SetActive(true);
    }
}
