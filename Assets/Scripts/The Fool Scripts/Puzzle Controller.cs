using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It manages the puzzle panels, their states, and the overall puzzle system in the game.
public class PuzzleController : MonoBehaviour
{
    public static PuzzleController Instance; // Singleton instance so it can be accessed globally.

    [Header("Puzzle Panels")]
    public GameObject[] puzzlePanels; // Array of puzzle panels to be managed.
    public GameObject puzzleCanvas; // The canvas that contains the puzzle panels.

    private static bool isPuzzleOpen = false; // Static variable to track if a puzzle is currently open.
    public static bool IsPuzzleOpen => isPuzzleOpen; // Property to access the puzzle open state.
    
    private bool[] puzzleCompletedStates; // Array to track the completion state of each puzzle.

    private float puzzleStartTime; // Time when the puzzle was started.

    public void Awake()
    {
        // Set up the singleton instance.
        if (Instance == null)
            Instance = this;

        // Ensure that the puzzle canvas is active at the start.
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Puzzle canvas is not assigned!", this);
        }

        // Initialize the puzzle complition array with the same length as the number of puzzle panels.
        puzzleCompletedStates = new bool[puzzlePanels.Length];
    }

    // Show a specified puzzle by index.
    public void ShowPuzzle(int index)
    {
        // Track puzzle attempt
        AnalyticsManager.Instance.TrackPuzzleAttempt(index, GetPuzzleType(index));
        puzzleStartTime = Time.unscaledTime;

        // Checks if the index is within bounds.
        if (index < 0 || index >= puzzlePanels.Length)
        {
            Debug.LogWarning($"Puzzle index {index} is out of range.");
            return;
        }

        // Doesn't open the puzzle panel if the puzzle is already completed.
        if (puzzleCompletedStates[index]) return;

        // Mark the puzzle as open and pause the game time.
        isPuzzleOpen = true;
        Time.timeScale = 0f;

        // Activate the puzzle canvas.
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(true);
        }

        // Enable the specified puzzle panel and disable all others.
        for (int i = 0; i < puzzlePanels.Length; i++)
        {
            puzzlePanels[i].SetActive(i == index);
        }
    }

    // Hides the puzzle panels.
    public void HidePuzzle()
    {
        isPuzzleOpen = false;

        // Makes sure the canvas is still enable.
        if (puzzleCanvas != null)
        {
            puzzleCanvas.SetActive(true);
        }

        // Turn off all puzzle panels.
        foreach (var panel in puzzlePanels)
        {
            panel.SetActive(false);
        }

        // Reusmes the game time if no other UI is open.
        if (!MenuController.IsMenuOpen && !InventoryUIController.IsInventoryOpen)
        {
            Time.timeScale = 1f;
        }
    }

    // Resets the puzzle open state.
    public void ResetPuzzleState()
    {
        isPuzzleOpen = false;
    }

    // Marks the puzzle as being complete and closes it.
    public void CompletePuzzle(int index)
    {
        // Calculate time taken
        float timeTaken = Time.unscaledTime - puzzleStartTime;

        // Track puzzle completion
        AnalyticsManager.Instance.TrackPuzzleComplete(
            index,
            GetPuzzleType(index),
            timeTaken
        );

        if (index >= 0 && index < puzzleCompletedStates.Length)
        {
            puzzleCompletedStates[index] = true;
            HidePuzzle();
        }
    }

    public void MarkPuzzleIncomplete(int index)
    {
        if (index >= 0 && index < puzzleCompletedStates.Length)
        {
            puzzleCompletedStates[index] = false;
            Debug.Log($"Puzzle {index} marked as incomplete.");
        }
        else
        {
            Debug.LogWarning($"MarkPuzzleIncomplete: Puzzle index {index} is out of range.");
        }
    }

    // Checks if a specific puzzle has been completed.
    public bool IsPuzzleCompleted(int index)
    {
        if (index >= 0 && index < puzzleCompletedStates.Length)
        {
            return puzzleCompletedStates[index];
        }

        return false;
    }

    // Returns the entire list of the puzzle completion states.
    public List<bool> GetPuzzleCompletionStates()
    {
        return new List<bool>(puzzleCompletedStates);
    }

    // Load saved puzzle completion states.
    public void SetPuzzleCompletionStates(List<bool> states)
    {
        if (states != null && states.Count == puzzlePanels.Length)
        {
            puzzleCompletedStates = states.ToArray();
        }
    }

    private string GetPuzzleType(int index)
    {
        // Return a string identifying the puzzle type
        return "puzzle_" + index;
    }
}
