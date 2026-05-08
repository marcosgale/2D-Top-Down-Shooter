// Bullet.cs
using UnityEngine;

/// <summary>
/// Controls the behavior of a bullet, including its movement and lifetime.
/// This script should be attached to the bullet prefab.
/// </summary>
public class Bullet : MonoBehaviour
{
    // --- Public Variables ---
    [Tooltip("The speed at which the bullet travels.")]
    public float speed = 20f;
    [Tooltip("The time in seconds before the bullet is automatically destroyed.")]
    public float destroyAfter = 3f;

    // --- Private Variables ---
    private Rigidbody2D rb;

    private void Awake()
    {
        // Get the Rigidbody2D component attached to the bullet.
        rb = GetComponent<Rigidbody2D>();

        // Destroy the bullet after a set amount of time to prevent clutter.
        Destroy(gameObject, destroyAfter);
    }

    private void FixedUpdate()
    {
        // Use FixedUpdate for physics-related movement.
        // Move the bullet forward based on its rotation.
        rb.linearVelocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet when it hits another object.
        // You would add more logic here, like damaging an enemy.
        Destroy(gameObject);
    }
}
