using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// This script is responsible for detecting player interactions with interactable objects in the game.
public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // The interactable object currently in range of the player.
    public GameObject interactionIcon; // The icon that will be displayed when the player can interact with an object.
    // Start is called before the first frame update
    void Start()
    {
        interactionIcon.SetActive(false); // Hide the interaction icon at the start.
    }

    public void OnInteract (InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact(); // Call the Interact method on the interactable object if it exists.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable; // Set the interactable object in range.
            interactionIcon.SetActive(true); // Show the interaction icon.
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null; // Clear the interactable object in range.
            interactionIcon.SetActive(false); // Hide the interaction icon.
        }
    }
}
