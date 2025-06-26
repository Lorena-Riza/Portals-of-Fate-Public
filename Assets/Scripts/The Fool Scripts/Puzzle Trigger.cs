using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Provides the trigger based logic to open and close the puzzle UI when the player interacts near a puzzle.
public class PuzzleTrigger : MonoBehaviour, IInteractable
{
    public int puzzleIndex; // Index of the puzzle this trigger is associated with.
    private bool playerInRange; // Flag to track if the player is in range of the trigger.
    public PlayerInput playerInput; // Reference to the PlayerInput component for handling input actions.

    // Called when another collider enters the trigger zone.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Checks if the entring object is the player.
        {
            playerInRange = true;
            // Subscribe to the input action for interaction.
            playerInput.actions["Interact"].performed += TogglePuzzle;
        }
    }

    // Called when another collider exits the trigger zone.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Checks if the exiting object is the player.
        {
            playerInRange = false;
            // Unsubscribe from the input action for interaction.
            playerInput.actions["Interact"].performed -= TogglePuzzle;
        }
    }

    // Called when the player interacts with the puzzle trigger.
    private void TogglePuzzle(InputAction.CallbackContext context)
    {
        // Do nothing if the player is not in range.
        if (!playerInRange) return;

        // Check if the puzzle is already completed.
        if (PuzzleController.Instance.IsPuzzleCompleted(puzzleIndex)) return;

        // If a puzzle is currently open, close it. Otherwise, open the puzzle with the specified index.
        if (PuzzleController.IsPuzzleOpen)
        {
            PuzzleController.Instance.HidePuzzle();
        }
        else
        {
            PuzzleController.Instance.ShowPuzzle(puzzleIndex);
        }
    }

    public void Interact()
    {
        
    }

    public bool CanInteract()
    {
        return !PuzzleController.Instance.IsPuzzleCompleted(puzzleIndex);
    }
}
