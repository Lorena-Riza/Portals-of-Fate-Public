using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Toggles the inventory UI on and off, using the new Unity Input System.
//It also pauses the game when the inventory is open and resumes it when closed.
public class InventoryUIController : MonoBehaviour
{
    public GameObject inventoryCanvas;//Reference to the inventory UI canvas.
    public PlayerInput playerInput;//Reference to the PlayerInput component.

    private static bool isInventoryOpen = false; //Variable to track if the inventory is open.
    public static bool IsInventoryOpen => isInventoryOpen;//Property to access the inventory state.

    //Validate that the inventoryCanvas and playerInput are assigned in the Inspector.
    private void Awake()
    {
        if (inventoryCanvas == null)
        {
            Debug.LogError($"[Awake] {gameObject.name} does not have inventoryCanvas assigned!", this);
        }

        if (playerInput == null)
        {
            Debug.LogError($"[Awake] {gameObject.name} does not have playerInput assigned!", this);
        }
    }

    private void Start()
    {
        Debug.Log($"[Start] inventoryController on {gameObject.name} with inventoryCanvas: {(inventoryCanvas == null ? "null" : inventoryCanvas.name)}", this);

        //Check if inventoryCanvas is assigned in the Inspector.
        if (inventoryCanvas == null)
        {
            Debug.LogError("inventoryCanvas is not assigned in the Inspector!", this);
            return;
        }
        else
        {
            Debug.Log("inventoryCanvas is assigned properly in the Inspector", this);
        }

        //Makes sure the inventory is closed at the start of the game.
        inventoryCanvas.SetActive(false);

        //Enables the PlayerInput component if it's assigned.
        if (playerInput != null)
        {
            Debug.Log("PlayerInput is properly assigned.");
            playerInput.enabled = true;
        }
    }

    //Subscribes to the inventory input action.
    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Inventory"].performed += ToggleInventory;
        }
        else
        {
            Debug.LogError("PlayerInput is not assigned in the Inspector!", this);
        }
    }

    //Unsubscribes from the inventory input action when the object is disabled.
    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Inventory"].performed -= ToggleInventory;
        }
    }

    //Unsubscribes from the inventory input action when the object is destroyed.
    private void OnDestroy()
    {
        if (playerInput != null)
        {
            Debug.Log("Unsubscribing from inventory input in OnDestroy", this);
            playerInput.actions["Inventory"].performed -= ToggleInventory;
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        //Ensure the object is valid before proceeding.
        if (this == null) return;

        Debug.Log($"Toggleinventory called from: {gameObject.name}", this);

        //Toggle the inventory state.
        isInventoryOpen = !isInventoryOpen;

        if (inventoryCanvas != null)
        {
            //Set the inventory canvas active or inactive based on the inventory state.
            inventoryCanvas.SetActive(isInventoryOpen);
        }
        else
        {
            Debug.LogError("inventoryCanvas is not assigned in the Inspector!", this);
        }

        //Pause or resume the game based on the inventory state.
        Time.timeScale = isInventoryOpen ? 0 : 1;
    }

    //Resets the inventory state to closed.
    public void ResetinventoryState()
    {
        isInventoryOpen = false;
    }
}