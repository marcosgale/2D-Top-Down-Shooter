using UnityEngine;
using UnityEngine.UIElements;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField, Tooltip("Radius around the player when using controller")]
    private float _controllerCrosshairRadius = 700f;

    private VisualElement _crosshair;
    private Vector2 _controllerAimDirection = Vector2.right; // Default direction
    private float _crosshairSize = 64f; // Size of the crosshair element

    private void OnEnable()
    {
        // Hide the system cursor when enabled
        UnityEngine.Cursor.visible = false;
    }

    private void Start()
    {
        // Hide the system cursor since we're using a custom crosshair
        UnityEngine.Cursor.visible = false;

        if (!_uiDocument)
            _uiDocument = GetComponent<UIDocument>();

        if (_uiDocument != null)
        {
            var root = _uiDocument.rootVisualElement;
            _crosshair = root.Q<VisualElement>("CrosshairContainer");
            
            // Get the actual size from the element if available
            if (_crosshair != null)
            {
                _crosshairSize = _crosshair.resolvedStyle.width;
            }
        }
    }

    private void Update()
    {
        if (_crosshair != null && PlayerCharacter.Instance != null)
        {
            // Get input mode and look input from PlayerCharacterMovement
            bool isUsingController = false;
            Vector2 lookInput = Vector2.zero;
            
            if (PlayerCharacter.Instance.CharacterMovement is PlayerCharacterMovement playerMovement)
            {
                isUsingController = playerMovement.IsUsingController;
                lookInput = playerMovement.LookInput;
                
                // Update controller aim direction when using controller and there's input
                if (isUsingController && lookInput.magnitude > 0.1f)
                {
                    _controllerAimDirection = lookInput.normalized;
                }
            }

            Vector3 screenPosition;

            if (isUsingController)
            {
                // Position crosshair at a fixed radius around the player
                Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(PlayerCharacter.Instance.transform.position);
                
                // Calculate offset based on controller direction
                Vector2 offset = _controllerAimDirection * _controllerCrosshairRadius;
                screenPosition = new Vector3(
                    playerScreenPos.x + offset.x,
                    playerScreenPos.y + offset.y,
                    0
                );
            }
            else
            {
                // Use mouse position
                screenPosition = Input.mousePosition;
            }

            // Use transform translate instead of left/top positioning
            // This bypasses layout system and directly positions the element
            float halfSize = 32f; // Half of 64px
            
            // Convert to UI Toolkit space (top-left origin)
            float uiX = screenPosition.x - halfSize;
            float uiY = Screen.height - screenPosition.y - halfSize;
            
            _crosshair.transform.position = new Vector3(uiX, uiY, 0);
        }
    }

    private void OnDestroy()
    {
        // Show the system cursor when this object is destroyed
        UnityEngine.Cursor.visible = true;
    }

    private void OnDisable()
    {
        // Show the system cursor when disabled
        UnityEngine.Cursor.visible = true;
    }
}