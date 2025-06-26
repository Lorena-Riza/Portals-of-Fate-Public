using System;
using UnityEngine;
using UnityEngine.UI;

// This script is responsible for managing the symbol slots in the puzzle.
public class SymbolSlot : MonoBehaviour
{
    public Image symbolImage; // The UI Image component that displays the current symbol sprite.
    private SymbolPuzzleManager puzzleManager; // Reference to the puzzle manager that owns this slot.
    private int currentSymbolIndex = 0; // The current index of the symbol being shown from the symbol list.

    // Initializes the symbol slot with a reference to the puzzle manager and sets the initial symbol.
    public void Initialize(SymbolPuzzleManager manager)
    {
        puzzleManager = manager;
        SetSymbol(0); // Default to first symbol.
    }

    // Sets the symbol image to the current symbol based on the index.
    public void OnClick()
    {
        // Cycle to the next symbol index (loop back to 0 at the end).
        currentSymbolIndex = (currentSymbolIndex + 1) % puzzleManager.symbols.Count;
        // Update the displayed symbol.
        SetSymbol(currentSymbolIndex);
        // Let the puzzle manager check if the entire combination is now correct.
        puzzleManager.CheckPuzzle();
    }

    // Sets the symbol sprite displayed in this slot based on the given index.
    public void SetSymbol(int index)
    {
        currentSymbolIndex = index;
        symbolImage.sprite = puzzleManager.symbols[currentSymbolIndex];
    }
    // Returns the current symbol index selected in this slot.
    public int GetSymbolIndex() => currentSymbolIndex;
}