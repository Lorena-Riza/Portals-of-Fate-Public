using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Toggles the pause menu UI such as the visibility of the menu canvas and the time scale of the game.
public class MainMenuController : MonoBehaviour
{
    public GameObject menuCanvas; //Reference to the menu canvas GameObject.
    public PlayerInput playerInput; //Reference to PlayerInput component.

    private static bool isMenuOpen = false; //Static variable to track if the menu is open.
    public static bool IsMenuOpen => isMenuOpen;

    private void Awake()
    {
        // Subscribe to scene load event to reset menu state when a new scene is loaded.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene load event to avoid memory leaks.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        //Ensure the menu canvas is inactive at the start.
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        //Force the menu to be closed at the start.
        ToggleMenu(default);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the menu state when a new scene is loaded.
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false); //Hide the canvas.
            isMenuOpen = false; //Update the flag.
            Time.timeScale = 1; //Resume the game time.
        }
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        //Toggle the open/close state of the menu.
        isMenuOpen = !isMenuOpen;

        //Apply visibility toggle to the menu canvas.
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(isMenuOpen);
        }
    }

    public void ResetMenuState()
    {
        //Reset the menu state.
        isMenuOpen = false;

        if (menuCanvas != null)
            menuCanvas.SetActive(false);
    }
}