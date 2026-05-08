using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _inputActionZoomCamera;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _smoothTime = 1.0f;

    [SerializeField]
    private float _zoomSpeed = 2.0f;

    [SerializeField]
    private float _minOrthographicSize = 5.0f;

    [SerializeField]
    private float _maxOrthographicSize = 20.0f;

    private float _zoomInput;

    private Vector3 _currentCameraVelocity;

    private void OnValidate()
    {
        if (!_camera)
            _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        // Read the input from the user.
        _zoomInput = _inputActionZoomCamera.action.ReadValue<float>();

        if (!PlayerCharacter.Instance)
            return;

        // Update the camera's position.
        Vector3 desiredPosition = transform.position;
        desiredPosition.x = PlayerCharacter.Instance.transform.position.x;
        desiredPosition.y = PlayerCharacter.Instance.transform.position.y;
        
        //transform.position = desiredPosition;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentCameraVelocity, _smoothTime);

        // Update orthographic size based on the zoom input.
        float orthographicSize = _camera.orthographicSize;

        orthographicSize += _zoomInput * _zoomSpeed * Time.deltaTime;
        orthographicSize = Mathf.Clamp(orthographicSize, _minOrthographicSize, _maxOrthographicSize);

        _camera.orthographicSize = orthographicSize;
    }
}
