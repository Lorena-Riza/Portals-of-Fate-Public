using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the logic for a chest puzzle in the game.
// The chests can either be truthful or deceptive.
// The player must choose the correct dialogue option to receive a reward.
// If the wrong option is chosen, the puzzle will take an item from the player.
// The player will need to redo the puzzle that gives him the item back.
public class ChestPuzzle : MonoBehaviour
{
    [Header("Chest Puzzle Settings")]
    public int choiceDialogueIndex; // Dialogue index for the choice
    public int trueAnswerIndex; // Index of the correct answer
    public bool isTruthChest = false; // Is this a truth chest?

    [Header("Item Info")]
    public GameObject rewardItemPrefab; // Item prefab to give as a reward.
    public GameObject itemToRemovePrefab; // Item prefab to remove from the player.

    public InventoryController inventoryController; // Reference to the InventoryController to manage items.

    [Header("Branch Dialogue")]
    public int noItemDialogueIndex;           // Dialogue if player lacks required item AND reward
    public int hasRewardAlreadyDialogueIndex; // Dialogue if player lacks required item but has reward

    [Header("Puzzle Info")]
    public int myPuzzleIndex; // Puzzle to mark complete when reward is given
    public int falseChestFailurePuzzleIndex; // Puzzle to mark incomplete on false chest wrong answer

    public bool IsChoiceDialogueIndex(int index)
    {
        return index == choiceDialogueIndex; // Check if the index matches the choice dialogue index
    }

    // Get the dialogue index based on the player's inventory state.
    public int GetDialogueIndexBasedOnInventory()
    {
        // Check if the inventory controller is assigned
        if (inventoryController == null)
        {
            Debug.LogWarning("[ChestPuzzle] InventoryController not assigned!");
            return -1;
        }

        bool hasItemToRemove = inventoryController.HasItem(itemToRemovePrefab); // Check if the player has the item to remove.
        bool hasRewardItem = rewardItemPrefab != null && inventoryController.HasItem(rewardItemPrefab); // Check if the player has the reward item.

        // Determine the dialogue index based on the player's inventory state.
        if (!hasItemToRemove && !hasRewardItem)
        {
            Debug.Log("[ChestPuzzle] Player does NOT have required item AND does NOT have reward item.");
            return noItemDialogueIndex;
        }
        else if (!hasItemToRemove && hasRewardItem)
        {
            // Player does not have the required item but already has the reward item.
            Debug.Log("[ChestPuzzle] Player does NOT have required item but ALREADY has reward item.");
            return hasRewardAlreadyDialogueIndex;
        }

        return -1; // Normal dialogue
    }

    /// Log the player's choice and handle the consequences.
    public void LogChoice(int chosenDialogueIndex)
    {
        AnalyticsManager.Instance.TrackPuzzleAttempt(myPuzzleIndex, "chest");

        bool isTrueChoice = chosenDialogueIndex == trueAnswerIndex;

        // Track chest choice
        AnalyticsManager.Instance.TrackChestChoice(isTruthChest, isTrueChoice);

        // Check if the chosen dialogue index matches the true answer index.
        if (isTruthChest)
        {
            Debug.Log("[ChestPuzzle] This chest is the TRUTH CHEST.");
        }
        else
        {
            Debug.Log("[ChestPuzzle] This chest is a FALSE CHEST.");
        }

        // Log the player's choice.
        if (isTrueChoice)
        {
            Debug.Log($"[ChestPuzzle] Player chose TRUE answer on {(isTruthChest ? "Truth Chest" : "False Chest")}.");
            // Check if the player has the item to remove and the inventory controller is assigned.
            if (itemToRemovePrefab != null && inventoryController != null)
            {
                // Check if the inventory controller is assigned and the item to remove prefab is not null.
                if (inventoryController.HasItem(itemToRemovePrefab))
                {
                    RemoveItemFromInventory(); // Remove the item from the inventory.
                    // Check if the player has the reward item prefab.
                    if (isTruthChest && rewardItemPrefab != null)
                    {
                        GiveItemToInventory(); // Give the reward item to the player.
                        PuzzleController.Instance.CompletePuzzle(myPuzzleIndex); // Mark the puzzle as complete.
                    }
                    else if (!isTruthChest)
                    {
                        // If it's a false chest, mark the puzzle as incomplete.
                        PuzzleController.Instance.MarkPuzzleIncomplete(falseChestFailurePuzzleIndex);
                        Debug.Log("[ChestPuzzle] FALSE chest: puzzle marked as incomplete due to wrong choice.");
                    }
                }
                else
                {
                    Debug.Log("[ChestPuzzle] Player does not have the required item, no reward or state change.");
                }
            }
            else
            {
                Debug.LogWarning("[ChestPuzzle] InventoryController not assigned or itemToRemovePrefab is null.");
            }
        }
        else
        {
            Debug.Log($"[ChestPuzzle] Player chose FALSE answer on {(isTruthChest ? "Truth Chest" : "False Chest")}.");
        }
    }

    /// Give the reward item to the player and remove the item from their inventory.
    private void GiveItemToInventory()
    {
        Item rewardItem = rewardItemPrefab.GetComponent<Item>();
        // Check if the reward item prefab has an Item component.
        if (rewardItem != null)
        {
            rewardItem.PickUp();
            inventoryController.AddItemToFirstEmptySlot(rewardItemPrefab); // Add the item to the inventory.
            Debug.Log("[ChestPuzzle] Reward item added to inventory.");
        }
        else
        {
            Debug.LogWarning("[ChestPuzzle] Reward item prefab has no Item component!");
        }
    }
    /// Remove the item from the player's inventory.
    private void RemoveItemFromInventory()
    {
        inventoryController.RemoveItem(itemToRemovePrefab);
        Debug.Log("[ChestPuzzle] Item removed from inventory.");
    }
}
