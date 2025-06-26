using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the access to door entry points and transitions based on the state of the door lock.
public class DoorLock : MonoBehaviour
{
    public bool isLocked = true; // The default state of the door lock is locked.
    public bool IsLocked => isLocked; // Read-only property to check if the door status.
    [SerializeField] private DoorEntry doorEntry; // Reference to the DoorEntry component.

    private bool isPlayerNear = false; // Tracks if the player is near the door.

    private void Start()
    {
        // Find the DoorEntry component on the same GameObject if not assigned in the inspector.
        if (doorEntry == null)
            doorEntry = GetComponent<DoorEntry>();
    }

    private void Update()
    {
        // Check if the player is near the door and skip the logic if not.
        if (!isPlayerNear) return;

        // If the player is near and interacts with the door, check if it isn't locked.
        if (UserInput.instance.InteractInput && !isLocked)
        {
            // Call MovePlayerToDoor directly since it's a public coroutine.
            StartCoroutine(doorEntry.MovePlayerToDoor());
        }
        // If the player is near and interacts with the door, but it is locked.
        else if (UserInput.instance.InteractInput && isLocked)
        {
            Debug.Log("The door is locked!");
        }
    }

    // Unlocks the door and allows the Player to enter.
    public void Unlock()
    {
        isLocked = false;
        Debug.Log("Door has been unlocked!");
    }

    // Locks the door to prevent usage.
    public void Lock()
    {
        isLocked = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enables interaction with the door when the player is near.
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Disable interaction when the player isn't within the trigger zone.
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
