using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script handles user input for a game character using Unity's new Input System.
// It works as a singleton to ensure only one instance of the input manager exists and reads values from a PlayerInput component.

public class UserInput : MonoBehaviour
{
    // Singleton instance
    public static UserInput instance;
    //Reference to PlayerInput component
    private PlayerInput playerInput;

    //public properties to access input values
    public bool MoveLeftInput { get; private set; }
    public bool MoveRightInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool InventoryInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool MenuInput { get; private set; }

    // Input actions for each control
    private InputAction moveLeftAction;
    private InputAction moveRightAction;
    private InputAction jumpAction;
    private InputAction inventoryAction;
    private InputAction interactAction;
    private InputAction menuAction;

    //InputClass is a generated class from the Input System
    private InputClass _inputMapping;

    private void Start()
    {
        // Makes sure that the PlayerInput component is assigned
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput was not assigned in Start(), retrying...");
            playerInput = GetComponent<PlayerInput>();
        }

        //Get the current control scheme
        if (playerInput != null)
        {
            Debug.Log("Current Control Scheme: " + playerInput.currentControlScheme);
        }
        else
        {
            Debug.LogError("PlayerInput is still null in Start()!");
        }
    }

    private void Awake()
    {
        //Singleton pattern to ensure only one instance of UserInput exists
        if (instance == null)
        {
            instance = this;
        }

        //Get the PlayerInput component
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing on " + gameObject.name);
        }

        //Initialize the InputClass
        _inputMapping = new InputClass();

        //Assign InputActions references
        SetupInputActions();

        //Enable the action map for the game
        EnableActionMap("Game");
    }

    //Enables the action map when the object becomes active
    private void OnEnable()
    {
        EnableActionMap("Game");
        _inputMapping?.Enable();
    }

    //Disables the action map when the object is disabled
    private void OnDisable()
    {
        DisableActionMap("Game");
        _inputMapping?.Disable();
    }

    //Sets up the input actions by assigning them to the corresponding variables
    private void SetupInputActions()
    {
        moveLeftAction = playerInput.actions["MoveLeft"];
        moveRightAction = playerInput.actions["MoveRight"];
        jumpAction = playerInput.actions["Jump"];
        inventoryAction = playerInput.actions["Inventory"];
        interactAction = playerInput.actions["Interact"];
        menuAction = playerInput.actions["Menu"];
    }

    //Is called once per frame to check and update the input values
    private void Update()
    {
        UpdateInput();
    }

    //Reads the input values from the InputActions and updates the public properties
    private void UpdateInput()
    {
        MoveLeftInput = moveLeftAction.ReadValue<float>() > 0;
        MoveRightInput = moveRightAction.ReadValue<float>() > 0;
        JumpInput = jumpAction.triggered;
        InventoryInput = inventoryAction.triggered;
        InteractInput = interactAction.triggered;
        MenuInput = menuAction.triggered;
    }

    //Enables the specified action map by name
    private void EnableActionMap(string actionMapName)
    {
        playerInput.actions.FindActionMap(actionMapName)?.Enable();
    }

    //Disables the specified action map by name
    private void DisableActionMap(string actionMapName)
    {
        playerInput.actions.FindActionMap(actionMapName)?.Disable();
    }
}