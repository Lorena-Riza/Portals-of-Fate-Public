using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resumes the game when the resume button is clicked.
public class ResumeButton : MonoBehaviour
{
    public GameObject menuCanvas; // Assign the menu canvas in the Inspector
    public MenuController menuController; // Reference to the MenuController

    public void Resume()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); // Hide the menu
            Time.timeScale = 1; // Resume the game

            if (menuController != null)
            {
                menuController.ResetMenuState(); // Reset the isMenuOpen state
            }
        }
        else
        {
            Debug.LogError("MenuCanvas is not assigned in the Inspector!", this);
        }
    }
}
