using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles interactions with the next scene trigger.
public class NextSceneInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextSceneName = "NextScene"; // The name of the next scene to load.
    [SerializeField] private InventoryController inventoryController; // The inventory controller to check for items.
    [SerializeField] private PlayerInput playerInput; // The player input to listen for interaction events.

    [SerializeField] private GameObject requiredItemPrefab; // The required item prefab to allow interaction.

    private bool playerInRange = false; // Flag to check if the player is in range to interact.

    private void Awake()
    {
        // Find the InventoryController.
        if (inventoryController == null)
        {
            inventoryController = FindObjectOfType<InventoryController>();
        }
        // Find the PlayerInput.
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
        }
        // Subscribe to the interact input action.
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed += OnInteractInput;
        }
        else
        {
            Debug.LogError("NextSceneInteract: PlayerInput not assigned or found.");
        }
    }

    // Unsubscribe from the interact input action when the object is destroyed.
    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed -= OnInteractInput;
        }
    }

    // Check if the player can interact with the object.
    public bool CanInteract()
    {
        if (!playerInRange) return false;

        if (requiredItemPrefab == null)
        {
            Debug.LogWarning("NextSceneInteract: No required item prefab assigned. Allowing interaction by default.");
            return true;
        }

        // InventoryController must be assigned to check for items.
        if (inventoryController == null)
        {
            Debug.LogWarning("NextSceneInteract: InventoryController not assigned!");
            return false;
        }

        // Uses the same HasItem method from inventoryController to check for the item.
        return inventoryController.HasItem(requiredItemPrefab);
    }

    // Perform the interaction.
    public void Interact()
    {
        if (!CanInteract()) return;

        Debug.Log("NextSceneInteract: Interacted. Clearing inventory and loading next scene.");

        ClearInventory();

        // Reset hint state before moving to the next scene
        HintGiver.ResetHintGiven(); // <<< Add this line

        // Set flag so SaveController skips loading saved data on next scene.
        PlayerPrefs.SetInt("LoadFromSave", 0);
        PlayerPrefs.Save();

        if (AnalyticsManager.Instance != null)
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            AnalyticsManager.Instance.TrackLevelComplete(currentSceneName, currentSceneIndex);
            Debug.Log($"Level complete event sent for scene: {currentSceneName}");
        }

        // Load the next scene using SceneController.
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadSceneWithTransition(nextSceneName);
        }
        else
        {
            Debug.LogError("SceneController instance is missing.");
        }
    }

    // Handle the input action for interaction.
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (playerInRange && CanInteract())
        {
            Interact();
        }
    }

    // Clear all items from the inventory.
    private void ClearInventory()
    {
        if (inventoryController == null || inventoryController.inventoryPanel == null) return;

        Transform panelTransform = inventoryController.inventoryPanel.transform;

        // Iterates through all slots in the inventory panel and destroys the current item.
        foreach (Transform slotTransform in panelTransform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }

        Debug.Log("All inventory items cleared.");
    }

    // Handle player entering and exiting the trigger area.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Handle player exiting the trigger area.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}