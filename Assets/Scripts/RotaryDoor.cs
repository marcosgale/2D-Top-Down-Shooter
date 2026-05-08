using UnityEngine;

public class RotaryDoor : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float openAngle = 90f;
    public float animationSpeed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * animationSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
