using System.Collections.Generic;
using UnityEngine;

// This script manages the symbol puzzle, including generating slots and checking the puzzle state.
public class SymbolPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public List<Sprite> symbols; // List of symbols to use in the puzzle.
    public List<int> correctCombination; // Correct combination of symbols (indices in the symbols list).
    public GameObject slotPrefab; // Prefab for the symbol slot.
    public Transform slotContainer; // Parent transform for the slots.

    private List<SymbolSlot> slots = new List<SymbolSlot>(); // List to hold the generated slots.

    [SerializeField] private int myPuzzleIndex; // Index of this puzzle in the PuzzleController.
    [SerializeField] private DoorLock doorToUnlock; // Door to unlock when the puzzle is completed.

    private float puzzleStartTime;

    // Start is called before the first frame update.
    private void Start()
    {
        puzzleStartTime = Time.unscaledTime;
        AnalyticsManager.Instance.TrackPuzzleAttempt(myPuzzleIndex, "symbol");
        GenerateSlots();
    }

    // Generates the slots for the puzzle based on the correct combination.
    void GenerateSlots()
    {
        // Destroy any existing children (used for puzzle reset or editor preview).
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        slots.Clear(); // Clear any references to old slots.

        // Create one slot per expected symbol in the correct combination.
        for (int i = 0; i < correctCombination.Count; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer); // Instantiate a new slot.
            SymbolSlot slot = slotObj.GetComponent<SymbolSlot>(); // Get the script attached to it.

            int correctIndex = correctCombination[i]; // The correct sprite index for this slot.
            int randomIndex; // Random index for the symbol to display.

            // If only one symbol exists, fallback to correct one to avoid infinite loop.
            if (symbols.Count <= 1)
            {
                randomIndex = correctIndex;
            }
            else
            {
                // Ensure we assign a different starting symbol (not the correct one).
                do
                {
                    randomIndex = Random.Range(0, symbols.Count);
                } while (randomIndex == correctIndex);
            }

            slot.Initialize(this); // Initialize the slot with this manager.
            slot.SetSymbol(randomIndex); // Set the initial symbol to a random one.
            slots.Add(slot); // Keep a reference to check later.
        }
    }

    // Checks if the current combination of symbols in the slots matches the correct combination.
    public void CheckPuzzle()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            // If any slot has the wrong symbol, puzzle is not complete.
            if (slots[i].GetSymbolIndex() != correctCombination[i])
                return;
        }

        float timeTaken = Time.unscaledTime - puzzleStartTime;
        AnalyticsManager.Instance.TrackPuzzleComplete(
            myPuzzleIndex,
            "symbol",
            timeTaken
        );

        Debug.Log("Puzzle Complete!");

        // Validate index before marking complete, to avoid out-of-bounds errors.
        if (myPuzzleIndex >= 0 && myPuzzleIndex < PuzzleController.Instance.GetPuzzleCompletionStates().Count)
        {
            PuzzleController.Instance.CompletePuzzle(myPuzzleIndex);
        }
        else
        {
            Debug.LogError($"Invalid puzzle index: {myPuzzleIndex}");
        }

        // Unlock the door if it's assigned.
        if (doorToUnlock != null)
        {
            doorToUnlock.Unlock();
        }
        else
        {
            Debug.LogWarning("No DoorLock assigned to unlock.");
        }
    }
}