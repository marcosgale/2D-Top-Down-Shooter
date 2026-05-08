using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpen = false;
    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    public RotaryDoor rotaryDoor;

    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            doorCollider.enabled = false; // disable blocking
            spriteRenderer.color = Color.green; // show visually it's open
            Debug.Log("Door opened!");
        }
    }
   

    public void CloseDoor()
    {
        if (isOpen)
        {
            isOpen = false;
            doorCollider.enabled = true;
            spriteRenderer.color = Color.red;
            Debug.Log("Door closed!");
        }
    }
}
