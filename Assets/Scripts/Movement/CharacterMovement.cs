using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidBody;
    public Rigidbody2D RigidBody => _rigidBody;

    [SerializeField]
    private bool _shouldUpdatePosition = true;
    public bool ShouldUpdatePosition
    {
        get => _shouldUpdatePosition;
        set => _shouldUpdatePosition = value;
    }

    [SerializeField]
    private bool _shouldUpdateRotation = true;
    public bool ShouldUpdateRotation
    {
        get => _shouldUpdateRotation;
        set => _shouldUpdateRotation = value;
    }

    [SerializeField]
    private float _baseMoveSpeed = 5.0f;
    public float BaseMoveSpeed
    {
        get => _baseMoveSpeed;
        set => _baseMoveSpeed = value < 0 ? 0 : value;
    }

    [SerializeField]
    private float _moveAcceleration = 1.0f;
    public float MoveAcceleration
    {
        get => _moveAcceleration;
        set => _moveAcceleration = value < 0 ? 0 : value;
    }

    [SerializeField]
    private float _rotateAcceleration = 360.0f;
    public float RotateAcceleration
    {
        get => _rotateAcceleration;
        set => _rotateAcceleration = value < 0 ? 0 : value;
    }

    [SerializeField]
    private AudioClipGroup _footStepClipGroup;
    public AudioClipGroup FootStepClipGroup => _footStepClipGroup;

    protected virtual void OnValidate()
    {
        if (!_rigidBody)
            _rigidBody = GetComponent<Rigidbody2D>();

        if (_baseMoveSpeed < 0.0f)
            _baseMoveSpeed = 0.0f;

        if (_moveAcceleration < 0.0f)
            _moveAcceleration = 0.0f;
    }

    private void FixedUpdate()
    {
        if (ShouldUpdatePosition)
            UpdatePosition();

        if (ShouldUpdateRotation)
            UpdateRotation();
    }

    protected virtual void UpdatePosition() {}
    protected virtual void UpdateRotation() {}

    /// <summary> Teleports the character to a location. </summary>
    public void TeleportTo(Vector2 worldPosition, float worldRotation)
    {
        RigidBody.position = worldPosition;
        RigidBody.rotation = worldRotation;
    }
}
