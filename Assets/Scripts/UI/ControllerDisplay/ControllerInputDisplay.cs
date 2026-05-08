using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ControllerInputDisplay : MonoBehaviour
{
    [SerializeField]
    private UIDocument _uiDocument;

    [SerializeField, Tooltip("Controller overlay sprite - use Assets/Sprites/controlleroverlay.jpg")]
    private Sprite _controllerSprite;

    [SerializeField, Tooltip("Size of the controller display in pixels")]
    private Vector2 _displaySize = new Vector2(250, 200);

    [SerializeField, Tooltip("Offset from bottom-left corner (below health bar)")]
    private Vector2 _cornerOffset = new Vector2(0, 110); // Position it below the 105px health bar + 5px gap

    [SerializeField, Tooltip("Color for active stick indicators")]
    private Color _activeColor = Color.green;

    [SerializeField, Tooltip("Color for inactive stick indicators")]
    private Color _inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [SerializeField, Tooltip("Dead zone for stick visualization")]
    private float _stickDeadZone = 0.25f;

    [SerializeField, Tooltip("Radius of stick movement visualization")]
    private float _stickVisualizationRadius = 30f;

    private VisualElement _controllerContainer;
    private VisualElement _controllerImage;
    private VisualElement _leftStickIndicator;
    private VisualElement _rightStickIndicator;
    
    // Button indicators
    private VisualElement _aButtonIndicator;
    private VisualElement _bButtonIndicator;
    private VisualElement _xButtonIndicator;
    private VisualElement _yButtonIndicator;
    private VisualElement _lbButtonIndicator;
    private VisualElement _rbButtonIndicator;
    private VisualElement _ltButtonIndicator;
    private VisualElement _rtButtonIndicator;
    
    private bool _isInitialized = false;

    private void Start()
    {
        if (!_uiDocument)
        {
            _uiDocument = GetComponent<UIDocument>();
            Debug.LogWarning("UIDocument was not assigned, attempting to get from component");
        }

        if (_uiDocument != null)
        {
            Debug.Log("UIDocument found, setting up controller display UI");
            SetupUI();
        }
        else
        {
            Debug.LogError("ControllerInputDisplay: No UIDocument found! Please assign a UIDocument component.");
        }
    }

    private void SetupUI()
    {
        var root = _uiDocument.rootVisualElement;
        Debug.Log($"SetupUI called. Root element exists: {root != null}, Root size: {root.resolvedStyle.width}x{root.resolvedStyle.height}");

        // Create main container
        _controllerContainer = new VisualElement();
        _controllerContainer.name = "ControllerDisplayContainer";
        _controllerContainer.style.position = Position.Absolute;
        _controllerContainer.style.width = _displaySize.x;
        _controllerContainer.style.height = _displaySize.y;
        _controllerContainer.style.left = _cornerOffset.x; // Position from left (below health bar)
        _controllerContainer.style.bottom = _cornerOffset.y;
        _controllerContainer.style.display = DisplayStyle.Flex; // Start visible for debugging

        Debug.Log($"Container created: size={_displaySize}, left={_cornerOffset.x}, bottom={_cornerOffset.y}");

        // Create controller image background
        _controllerImage = new VisualElement();
        _controllerImage.name = "ControllerImage";
        _controllerImage.style.width = Length.Percent(100);
        _controllerImage.style.height = Length.Percent(100);
        _controllerImage.style.position = Position.Relative; // Changed from Absolute
        
        Debug.Log($"Controller sprite null: {_controllerSprite == null}");
        
        if (_controllerSprite != null)
        {
            _controllerImage.style.backgroundImage = new StyleBackground(_controllerSprite);
            _controllerImage.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            Debug.Log($"Using controller sprite: {_controllerSprite.name}");
        }
        else
        {
            // Fallback: semi-transparent dark background with visible border
            _controllerImage.style.backgroundColor = new Color(0.2f, 0.2f, 0.25f, 0.9f);
            _controllerImage.style.borderTopLeftRadius = 10;
            _controllerImage.style.borderTopRightRadius = 10;
            _controllerImage.style.borderBottomLeftRadius = 10;
            _controllerImage.style.borderBottomRightRadius = 10;
            _controllerImage.style.borderTopWidth = 3;
            _controllerImage.style.borderBottomWidth = 3;
            _controllerImage.style.borderLeftWidth = 3;
            _controllerImage.style.borderRightWidth = 3;
            _controllerImage.style.borderTopColor = new Color(0.6f, 0.6f, 0.7f, 1f);
            _controllerImage.style.borderBottomColor = new Color(0.6f, 0.6f, 0.7f, 1f);
            _controllerImage.style.borderLeftColor = new Color(0.6f, 0.6f, 0.7f, 1f);
            _controllerImage.style.borderRightColor = new Color(0.6f, 0.6f, 0.7f, 1f);
            Debug.LogWarning("No controller sprite assigned! Using fallback background.");
        }

        // Create left stick indicator (positioned on upper-left stick, fine-tuned)
        _leftStickIndicator = CreateStickIndicator("LeftStickIndicator", 23, 29);
        
        // Create right stick indicator (positioned on lower-right stick, adjusted)
        _rightStickIndicator = CreateStickIndicator("RightStickIndicator", 61, 49);

        // Create button indicators (face buttons on right side - equidistant diamond pattern)
        _yButtonIndicator = CreateButtonIndicator("YButton", 73, 24); // Y button - top (raised 2px)
        _aButtonIndicator = CreateButtonIndicator("AButton", 73, 40); // A button - bottom (unchanged)
        _bButtonIndicator = CreateButtonIndicator("BButton", 80, 31); // B button - right (raised 2px)
        _xButtonIndicator = CreateButtonIndicator("XButton", 66, 31); // X button - left (raised 2px)
        
        // Create shoulder button indicators
        _lbButtonIndicator = CreateButtonIndicator("LBButton", 18, 8); // Left trigger (top left)
        _rbButtonIndicator = CreateButtonIndicator("RBButton", 82, 8); // Right trigger (top right)
        _ltButtonIndicator = CreateButtonIndicator("LTButton", 14, 14); // Left bumper (second from top left)
        _rtButtonIndicator = CreateButtonIndicator("RTButton", 86, 14); // Right bumper (second from top right)

        // Assemble hierarchy
        _controllerContainer.Add(_controllerImage);
        _controllerContainer.Add(_leftStickIndicator);
        _controllerContainer.Add(_rightStickIndicator);
        _controllerContainer.Add(_aButtonIndicator);
        _controllerContainer.Add(_bButtonIndicator);
        _controllerContainer.Add(_xButtonIndicator);
        _controllerContainer.Add(_yButtonIndicator);
        _controllerContainer.Add(_lbButtonIndicator);
        _controllerContainer.Add(_rbButtonIndicator);
        _controllerContainer.Add(_ltButtonIndicator);
        _controllerContainer.Add(_rtButtonIndicator);
        root.Add(_controllerContainer);

        Debug.Log($"Elements added to hierarchy. Container children: {_controllerContainer.childCount}, Root children: {root.childCount}");

        _isInitialized = true;
        Debug.Log($"ControllerInputDisplay initialized successfully. Root has {root.childCount} children. Container added at position: left={_cornerOffset.x}px, bottom={_cornerOffset.y}px, size={_displaySize}");
        Debug.Log($"Container display style: {_controllerContainer.style.display.value}, Visible in hierarchy: {_controllerContainer.visible}");
    }

    private VisualElement CreateStickIndicator(string name, float leftPercent, float topPercent)
    {
        var indicator = new VisualElement();
        indicator.name = name;
        indicator.style.position = Position.Absolute;
        indicator.style.width = 20;
        indicator.style.height = 20;
        indicator.style.left = Length.Percent(leftPercent);
        indicator.style.top = Length.Percent(topPercent);
        indicator.style.backgroundColor = _inactiveColor;
        indicator.style.borderTopLeftRadius = 10;
        indicator.style.borderTopRightRadius = 10;
        indicator.style.borderBottomLeftRadius = 10;
        indicator.style.borderBottomRightRadius = 10;
        indicator.style.borderTopWidth = 2;
        indicator.style.borderBottomWidth = 2;
        indicator.style.borderLeftWidth = 2;
        indicator.style.borderRightWidth = 2;
        indicator.style.borderTopColor = Color.white;
        indicator.style.borderBottomColor = Color.white;
        indicator.style.borderLeftColor = Color.white;
        indicator.style.borderRightColor = Color.white;

        return indicator;
    }

    private VisualElement CreateButtonIndicator(string name, float leftPercent, float topPercent)
    {
        var button = new VisualElement();
        button.name = name;
        button.style.position = Position.Absolute;
        button.style.width = 15;
        button.style.height = 15;
        button.style.left = Length.Percent(leftPercent);
        button.style.top = Length.Percent(topPercent);
        button.style.backgroundColor = new Color(1f, 1f, 1f, 0.3f); // Semi-transparent white when inactive
        button.style.borderTopLeftRadius = 8;
        button.style.borderTopRightRadius = 8;
        button.style.borderBottomLeftRadius = 8;
        button.style.borderBottomRightRadius = 8;

        return button;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }
        
        if (PlayerCharacter.Instance == null)
            return;

        if (PlayerCharacter.Instance.CharacterMovement is PlayerCharacterMovement playerMovement)
        {
            bool isUsingController = playerMovement.IsUsingController;
            
            // Show/hide the display based on input mode
            DisplayStyle targetDisplay = isUsingController ? DisplayStyle.Flex : DisplayStyle.None;
            if (_controllerContainer.style.display.value != targetDisplay)
            {
                _controllerContainer.style.display = targetDisplay;
                Debug.Log($"Controller display {(isUsingController ? "SHOWN" : "HIDDEN")} - IsUsingController={isUsingController}");
            }

            if (isUsingController)
            {
                // Get input from PlayerInGameInput
                Vector2 moveInput = playerMovement.MoveInput;
                Vector2 lookInput = playerMovement.LookInput;

                // Update left stick (movement) - fine-tuned position
                UpdateStickIndicator(_leftStickIndicator, moveInput, 23, 29);
                
                // Update right stick (aiming) - adjusted position
                UpdateStickIndicator(_rightStickIndicator, lookInput, 61, 49);
                
                // Update button indicators based on input
                UpdateButtonIndicator(_aButtonIndicator, Gamepad.current != null && Gamepad.current.buttonSouth.isPressed); // A button - Jump
                UpdateButtonIndicator(_bButtonIndicator, Gamepad.current != null && Gamepad.current.buttonEast.isPressed); // B button - Crouch
                UpdateButtonIndicator(_xButtonIndicator, Gamepad.current != null && Gamepad.current.buttonWest.isPressed); // X button - Reload
                UpdateButtonIndicator(_yButtonIndicator, Gamepad.current != null && Gamepad.current.buttonNorth.isPressed); // Y button - Interact
                UpdateButtonIndicator(_lbButtonIndicator, Gamepad.current != null && Gamepad.current.dpad.left.isPressed); // LB - Previous Weapon (D-pad left)
                UpdateButtonIndicator(_rbButtonIndicator, Gamepad.current != null && Gamepad.current.rightShoulder.isPressed); // RB - Throw Grenade
                UpdateButtonIndicator(_ltButtonIndicator, Gamepad.current != null && Gamepad.current.leftTrigger.isPressed); // LT - Secondary Attack
                UpdateButtonIndicator(_rtButtonIndicator, Gamepad.current != null && Gamepad.current.rightTrigger.isPressed); // RT - Attack
            }
        }
    }

    private void UpdateButtonIndicator(VisualElement button, bool isPressed)
    {
        if (isPressed)
        {
            button.style.backgroundColor = new Color(0f, 1f, 0f, 0.9f); // Bright green when pressed
        }
        else
        {
            button.style.backgroundColor = new Color(1f, 1f, 1f, 0.3f); // Semi-transparent white when not pressed
        }
    }

    private void UpdateStickIndicator(VisualElement indicator, Vector2 input, float baseLeftPercent, float baseTopPercent)
    {
        float magnitude = input.magnitude;
        
        // Update color based on whether stick is active
        if (magnitude > _stickDeadZone)
        {
            indicator.style.backgroundColor = _activeColor;
            
            // Calculate offset based on input (normalized and scaled)
            Vector2 normalizedInput = input.normalized;
            float offsetX = normalizedInput.x * _stickVisualizationRadius;
            float offsetY = -normalizedInput.y * _stickVisualizationRadius; // Invert Y for UI coordinates
            
            // Apply offset while maintaining base position
            indicator.style.left = new StyleLength(new Length(baseLeftPercent, LengthUnit.Percent));
            indicator.style.top = new StyleLength(new Length(baseTopPercent, LengthUnit.Percent));
            indicator.style.translate = new StyleTranslate(new Translate(offsetX, offsetY));
        }
        else
        {
            indicator.style.backgroundColor = _inactiveColor;
            indicator.style.left = new StyleLength(new Length(baseLeftPercent, LengthUnit.Percent));
            indicator.style.top = new StyleLength(new Length(baseTopPercent, LengthUnit.Percent));
            indicator.style.translate = new StyleTranslate(new Translate(0, 0));
        }
    }
}
