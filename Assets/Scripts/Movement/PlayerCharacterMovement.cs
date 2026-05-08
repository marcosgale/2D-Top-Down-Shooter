using UnityEngine;

public class PlayerCharacterMovement : CharacterMovement
{
    [SerializeField] private float _sprintSpeed = 10.0f;
    [SerializeField] private float _controllerLookSensitivity = 200.0f;
    [SerializeField] private float _controllerDeadZone = 0.25f; // Dead zone for stick drift
    [SerializeField] private float _inputToggleHoldTime = 2.0f; // Time to hold minus key to toggle
    
    [HideInInspector] public Vector2 MoveInput = Vector2.zero;
    [HideInInspector] public Vector2 LookInput = Vector2.zero;
    [HideInInspector] public bool IsSprinting = false;

    private float _currentRotateSpeed;
    private bool _isUsingController = false;
    private float _minusKeyHoldTime = 0f;
    
    // Public property for other systems to read input mode state
    public bool IsUsingController => _isUsingController;

    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f; 

    protected override void UpdatePosition()
    {
        Vector2 desiredVelocity = MoveInput.normalized * (IsSprinting ? _sprintSpeed : BaseMoveSpeed);
        RigidBody.linearVelocity = Vector2.MoveTowards(RigidBody.linearVelocity, desiredVelocity, MoveAcceleration * Time.deltaTime);

        if (MoveInput.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                //SoundEffectManager.Play("Footstep");
                AudioSourcePool.Instance.PlayOneShot(transform.position, FootStepClipGroup.GetRandomAudioClip());
                footstepTimer = footstepInterval;
            }
        }
    }

    protected override void UpdateRotation()
    {
        // Check for toggle button hold (minus key)
        if (Input.GetKey(KeyCode.Minus))
        {
            _minusKeyHoldTime += Time.deltaTime;
            
            if (_minusKeyHoldTime >= _inputToggleHoldTime)
            {
                _isUsingController = !_isUsingController;
                Debug.Log($"Input mode switched to: {(_isUsingController ? "Controller" : "Mouse")}");
                _minusKeyHoldTime = 0f; // Reset to prevent repeated toggles
            }
        }
        else
        {
            _minusKeyHoldTime = 0f; // Reset if key is released
        }

        float desiredRotation;

        // Use the active input method
        if (_isUsingController)
        {
            // Controller mode: use right stick input
            if (LookInput.magnitude > _controllerDeadZone)
            {
                // Use controller right stick as direct axis - stick direction = facing direction
                desiredRotation = Mathf.Atan2(LookInput.y, LookInput.x) * Mathf.Rad2Deg;
                
                // Smoothly rotate to the stick direction
                RigidBody.rotation = Mathf.MoveTowardsAngle(RigidBody.rotation, desiredRotation, _controllerLookSensitivity * Time.deltaTime);
            }
            // If stick is within dead zone, don't update rotation (keep current rotation)
        }
        else
        {
            // Mouse mode: use cursor position
            Vector3 cursorPosition = Input.mousePosition;
            cursorPosition.z = Camera.main.farClipPlane;
            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

            Vector3 directionToCursor = cursorWorldPosition - transform.position;
            directionToCursor.z = 0.0f;

            desiredRotation = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;
            
            RigidBody.rotation = Mathf.SmoothDampAngle(RigidBody.rotation, desiredRotation, ref _currentRotateSpeed, 0.02f);
        }
    }
}
