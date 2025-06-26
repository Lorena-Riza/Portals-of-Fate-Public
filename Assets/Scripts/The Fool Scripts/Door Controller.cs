using System.Collections.Generic;
using UnityEngine;

// This script manages all locked doors in the game.
// It allows querying and restoring their lock states,
// useful for save/load systems or persistent world behavior.
public class DoorController : MonoBehaviour
{
    // Singleton instance so the DoorController can be accessed globally.
    public static DoorController Instance { get; private set; }

    [Header("Locked Doors List (drag in DoorLock scripts here)")]
    public List<DoorLock> lockedDoors = new List<DoorLock>();

    private void Awake()
    {
        // Ensure only one instance of DoorController exists.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // If another instance exists, destroy this one.
            Destroy(gameObject);
        }
    }

    // Returns the lock state of each door in the list.
    // true = locked, false = unlocked.
    public List<bool> GetDoorLockStates()
    {
        List<bool> states = new List<bool>();

        foreach (var door in lockedDoors)
        {
            if (door != null)
            {
                // Add the door's current lock state to the list.
                states.Add(door.IsLocked);
            }
            else
            {
                Debug.LogWarning("DoorController: One of the locked doors is missing!");
                states.Add(true); // default to locked if null.
            }
        }

        return states;
    }

    // Set door states based on manual list order.
    public void SetDoorLockStates(List<bool> savedStates)
    {
        // Check if the state list is valid and matches the number of tracked doors.
        if (savedStates == null || savedStates.Count != lockedDoors.Count)
        {
            Debug.LogWarning("DoorController: Saved door states count doesn't match the number of tracked doors.");
            return;
        }

        // Apply the saved lock/unlock state to each door.
        for (int i = 0; i < lockedDoors.Count; i++)
        {
            if (lockedDoors[i] == null) continue;

            if (savedStates[i])
                lockedDoors[i].Lock();
            else
                lockedDoors[i].Unlock();
        }

        Debug.Log("DoorController: Door states restored from saved data.");
    }
}