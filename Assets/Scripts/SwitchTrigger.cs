using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public Door connectedDoor; // drag the door here in Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() != null)
        {
            Debug.Log("Player activated the switch!");
            connectedDoor.rotaryDoor.ToggleDoor();
        }
    }
}
