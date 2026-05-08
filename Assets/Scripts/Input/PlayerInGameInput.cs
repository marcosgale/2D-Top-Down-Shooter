using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class InputData
{
    public Vector2 MoveInput;
    public Vector2 LookInput;
    public bool IsFireButtonPressed;
    public bool IsReloadButtonPressed;
    public bool IsMeleeButtonPressed;
    public bool IsSprintButtonPressed;
    public bool IsThrowGrenadeButtonPressed;
}

public class PlayerInGameInput : MonoBehaviour
{
    [SerializeField]
    private string _inGameActionMap = "Player";

    [SerializeField]
    private InputActionReference _inputActionMove;

    [SerializeField]
    private InputActionReference _inputActionLook;

    [SerializeField]
    private InputActionReference _inputActionSprint;

    [SerializeField]
    private InputActionReference _inputActionFire;

    [SerializeField]
    private InputActionReference _inputActionReload;

    [SerializeField]
    private InputActionReference _inputActionMeleeAttack;

    [SerializeField]
    private InputActionReference _inputActionThrowGrenade;

    private InputData _inputData;
    public InputData InputData => _inputData;

    private void Awake()
    {
        _inputData = new InputData();
    }

    private void Start()
    {
        PlayerInput.all[0].SwitchCurrentActionMap(_inGameActionMap);
    }

    private void Update()
    {
        _inputData.MoveInput = _inputActionMove.action.ReadValue<Vector2>();
        _inputData.LookInput = _inputActionLook != null ? _inputActionLook.action.ReadValue<Vector2>() : Vector2.zero;
        
        _inputData.IsFireButtonPressed = _inputActionFire.action.IsPressed();
        _inputData.IsReloadButtonPressed = _inputActionReload.action.WasPressedThisFrame();
        _inputData.IsMeleeButtonPressed = _inputActionMeleeAttack.action.WasPressedThisFrame();
        _inputData.IsSprintButtonPressed = _inputActionSprint.action.IsPressed();
        _inputData.IsThrowGrenadeButtonPressed = _inputActionThrowGrenade.action.WasPressedThisFrame();
    }
}
