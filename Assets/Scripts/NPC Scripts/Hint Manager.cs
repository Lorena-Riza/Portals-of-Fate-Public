using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class to manage hint state.
public class HintManager : MonoBehaviour
{
    // Singleton instance of HintManager.
    public static HintManager Instance { get; private set; }
    // Flag to indicate if a hint has been given.
    public bool HintGiven { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of HintManager exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance.
            return;
        }
        Instance = this;
    }

    // Mark that a hint has been given (called when a player selects a hint).
    public void MarkHintGiven()
    {
        HintGiven = true;
    }

    // Reset hint status.
    public void ResetHintFlag()
    {
        HintGiven = false;
    }
}
