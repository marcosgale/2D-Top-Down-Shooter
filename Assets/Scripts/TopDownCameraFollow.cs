using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;           // Player to follow
    public float smoothSpeed = 0.125f; // Smoothing factor
    public Vector3 offset = new Vector3(0, 0, -10); // Keeps camera above the scene

    void LateUpdate()
    {
        if (target == null) return;

        // Only follow X and Y; keep camera's Z fixed
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
