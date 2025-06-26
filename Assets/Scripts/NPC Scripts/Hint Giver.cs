using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to NPCs that can give hints to the player via dialogue.
public class HintGiver : MonoBehaviour
{
    public int hintChoiceIndex; // The index of the choice that gives a hint.

    public int[] hintResponseIndices; // The indices of the choices that are responses to the hint.

    public int endLineIndex; // The index of the line that ends the dialogue.

    private static bool hintGivenThisLevel = false; // Static variable to track if a hint has been given in the current level.
    public static bool HintGiven => hintGivenThisLevel; // Property to check if a hint has been given.

    // Resets the hint given status for the current level.
    public static void ResetHintGiven()
    {
        hintGivenThisLevel = false;
    }

    // Checks if the given index is the hint choice index.
    public bool IsHintChoiceIndex(int index)
    {
        return !hintGivenThisLevel && index == hintChoiceIndex;
    }

    // Checks if the given index is a response to the hint.
    public void MarkHintGiven(int chosenIndex)
    {
        foreach (int i in hintResponseIndices)
        {
            if (i == chosenIndex)
            {
                hintGivenThisLevel = true; // Mark the current hint as given for this level.
                return;
            }
        }
    }

    // Checks if the given index is the end line index.
    public int GetEndLine()
    {
        return endLineIndex;
    }

    // Sets the hint given status for the current level.
    public static void SetHintGiven(bool value)
    {
        hintGivenThisLevel = value;
    }
}