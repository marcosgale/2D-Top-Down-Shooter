using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private string _pickupTag = "Player"; // Tag of object that can pick this up

    [SerializeField]
    private float _rotationSpeed = 90f; // Optional: for visual flair

    [SerializeField]
    private AudioClip _pickupSound;

    private void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_pickupTag))
        {
            Debug.Log($"{gameObject.name} picked up by {other.name}");

            // Optional: trigger effect or apply item logic here
            if (_pickupSound != null)
                AudioSource.PlayClipAtPoint(_pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}
