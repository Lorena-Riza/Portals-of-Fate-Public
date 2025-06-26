using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//This class is responsible for adding an item to the player's inventory when they interact with a specific trigger.
public class AddItemToInventory : MonoBehaviour
{
    public int puzzleIndex; // The index of the puzzle this script is for.
    private bool playerInRange; // To check if the player is in range of the trigger.
    public PlayerInput playerInput; // Reference to the PlayerInput for detecting interact button press.

    [Header("Inventory Settings")]
    [SerializeField] private InventoryController inventoryController; // The InventoryController reference.
    [SerializeField] private GameObject inventoryItemPrefab; // The item prefab to add to the inventory.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInput.actions["Interact"].performed += OnInteract; // Bind the interact action to the OnInteract method.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInput.actions["Interact"].performed -= OnInteract; // Unbind the interact action.
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Item item = inventoryItemPrefab.GetComponent<Item>(); // Get the Item component from the prefab.
        if (!playerInRange) return; // Ensure the player is in range of the trigger.

        // Do nothing if the puzzle is already completed.
        if (PuzzleController.Instance.IsPuzzleCompleted(puzzleIndex)) return;

        // Add item to inventory.
        if (inventoryController != null && inventoryItemPrefab != null)
        {
            item.PickUp(); // Call the PickUp method on the item.
            inventoryController.AddItemToFirstEmptySlot(inventoryItemPrefab); // Adds the item to the first empty slot.
            Debug.Log("Item added to inventory.");
        }

        // Mark the puzzle as completed and hide it.
        PuzzleController.Instance.CompletePuzzle(puzzleIndex);
        PuzzleController.Instance.HidePuzzle();
    }
}
