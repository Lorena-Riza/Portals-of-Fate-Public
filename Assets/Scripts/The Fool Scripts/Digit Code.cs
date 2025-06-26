using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This code handles the functionality of a digit code puzzle in a game.
// Displays the digit input from buttons.
// It handles the clear and submit actions.
// It checks if the entered code matches the correct code.
// It notifies the PuzzleController when the puzzle is completed.
public class DigitCode : MonoBehaviour
{
    public TMP_Text codeDisplay; // References the UI text element to display the entered code.
    private string currentInput = ""; // Stores the current input from the player.

    public string correctCode = "1234"; // The correct code to be entered by the player.

    [SerializeField] private int myPuzzleIndex; // The index of the puzzle associated with this digit code.

    [Header("Inventory Settings")]
    [SerializeField] private InventoryController inventoryController; // The InventoryController reference.
    [SerializeField] private GameObject inventoryItemPrefab; // The item prefab to add to the inventory.

    private float puzzleStartTime;

    private void Awake()
    {
        puzzleStartTime = Time.unscaledTime;
        AnalyticsManager.Instance.TrackPuzzleAttempt(myPuzzleIndex, "digit_code");
    }

    /// Called when a digit button is pressed.
    public void OnDigitPressed(string digit)
    {
        // Prevent input beyond the length of the correct code.
        if (currentInput.Length >= correctCode.Length)
            return;

        currentInput += digit; // Add the pressed digit to the current input.
        UpdateDisplay(); // Update the display with the current input.
    }

    /// Called when the clear button is pressed.
    public void OnClearPressed()
    {
        currentInput = ""; // Clear the current input.
        UpdateDisplay(); // Update the display to show the cleared input.
    }

    /// Called when the submit button is pressed.
    public void OnSubmitPressed()
    {
        // Check if the current input matches the correct code.
        if (currentInput == correctCode)
        {
            float timeTaken = Time.unscaledTime - puzzleStartTime;
            AnalyticsManager.Instance.TrackPuzzleComplete(
                myPuzzleIndex,
                "digit_code",
                timeTaken
            );

            // Mark puzzle as completed and hide UI.
            PuzzleController.Instance.CompletePuzzle(myPuzzleIndex);
            PuzzleController.Instance.HidePuzzle();

            Item item = inventoryItemPrefab.GetComponent<Item>(); // Get the Item component from the prefab.

            item.PickUp(); // Call the PickUp method on the item.
            inventoryController.AddItemToFirstEmptySlot(inventoryItemPrefab); // Adds the item to the first empty slot.
            Debug.Log("Item added to inventory.");

            gameObject.SetActive(false); // Disable keypad panel.
        }
        else
        {
            // Incorrect code entered.
            Debug.Log("Incorrect code.");
            currentInput = "";
            UpdateDisplay();
        }
    }

    /// Updates the display text with the current input.
    private void UpdateDisplay()
    {
        codeDisplay.text = currentInput;
    }
}