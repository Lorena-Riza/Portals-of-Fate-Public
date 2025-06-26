using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//This script controls the menu system in the game.
//It opens and closes the menu when the player presses the assigned input key.
//Pauses and resumes the game time when the menu is opened or closed.
//Ensures correct input binding and menu state management.
public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas; //The canvas that contains the menu UI elements.
    public PlayerInput playerInput; //The PlayerInput component that handles input actions.

    private static bool isMenuOpen = false; //Tracks the state of the menu (open or closed).
    public static bool IsMenuOpen => isMenuOpen;

    //Ensures the menuCanvas and playerInput are assigned in the Inspector.
    private void Awake()
    {
        if (menuCanvas == null)
        {
            Debug.LogError($"[Awake] {gameObject.name} does not have menuCanvas assigned!", this);
        }

        if (playerInput == null)
        {
            Debug.LogError($"[Awake] {gameObject.name} does not have playerInput assigned!", this);
        }
    }

    private void Start()
    {
        Debug.Log($"[Start] MenuController on {gameObject.name} with menuCanvas: {(menuCanvas == null ? "null" : menuCanvas.name)}", this);

        if (menuCanvas == null)
        {
            Debug.LogError("menuCanvas is not assigned in the Inspector!", this);
            return;
        }
        else
        {
            Debug.Log("menuCanvas is assigned properly in the Inspector", this);
        }

        //Ensure the menu is closed at the start of the game.
        menuCanvas.SetActive(false);

        //Enable the PlayerInput component to receive input events.
        if (playerInput != null)
        {
            Debug.Log("PlayerInput is properly assigned.");
            playerInput.enabled = true;
        }
    }

    //Subscribe to the Menu input action when the object is enabled.
    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Menu"].performed += ToggleMenu;
        }
        else
        {
            Debug.LogError("PlayerInput is not assigned in the Inspector!", this);
        }
    }

    //Unsubscribe from the Menu input action when the object is disabled.
    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Menu"].performed -= ToggleMenu;
        }
    }

    //Clean up the input subscription when the object is destroyed.
    private void OnDestroy()
    {
        if (playerInput != null)
        {
            Debug.Log("Unsubscribing from Menu input in OnDestroy", this);
            playerInput.actions["Menu"].performed -= ToggleMenu;
        }
    }

    //Called when the menu input action is performed.
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        //Check if the menuCanvas is null before proceeding.
        if (this == null) return;

        Debug.Log($"ToggleMenu called from: {gameObject.name}", this);

        //Flip the menu state.
        isMenuOpen = !isMenuOpen;

        //If the menu is open, pause the game and show the menu.
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(isMenuOpen);
        }
        else
        {
            Debug.LogError("menuCanvas is not assigned in the Inspector!", this);
        }

        //Pause or resume the game time based on the menu state.
        Time.timeScale = isMenuOpen ? 0 : 1;
    }

    //Called when the player clicks the "Resume" button in the menu.
    public void ResetMenuState()
    {
        isMenuOpen = false;
    }
}