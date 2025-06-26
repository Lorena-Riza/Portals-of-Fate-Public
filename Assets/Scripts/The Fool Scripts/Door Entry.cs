using Cinemachine;
using System.Collections;
using UnityEngine;

//This script handles the door entry and exit functionality in the game.
// It allows the player to transition between different areas of the game world by moving them to a specified exit point.
public class DoorEntry : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform exitPoint; // The target position where the player will be teleported once they enter the door.
    [SerializeField] private PolygonCollider2D newMapBoundary; // The new map boundary that will be set when the player enters the door.
    [SerializeField] private Animator transitionAnimation; // The animator component that controls the transition animation when the player enters the door.

    private bool isPlayerNear; // Tracks if the player is near the door.
    private GameObject player; // Reference to the player GameObject.
    private CinemachineVirtualCamera cinemachineCam; // Reference to the Cinemachine virtual camera.
    private CinemachineConfiner confiner; // Reference to the Cinemachine confiner component that restricts the camera's movement to a specific area.
    private Rigidbody2D playerRb; // Reference to the player's Rigidbody2D component.

    private static bool isTransitioning = false; //Flag to prevent multiple transitions at the same time.
    private AudioManager audioManager; // Reference to the AudioManager component for playing sound effects.

    private void Start()
    {
        // Initialize references to the player, cinemachine camera, confiner, and audio manager.
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        confiner = FindObjectOfType<CinemachineConfiner>();

        if (player != null)
        {
            // Get the Rigidbody2D component of the player if it exists.
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        // Find the AudioManager in the scene.
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        // Check if the player is near the door and if they are pressing the interact button and no transiition is in progress.
        if (isPlayerNear && UserInput.instance.InteractInput && !isTransitioning)
        {
            // Check if the door is locked.
            DoorLock doorLock = GetComponent<DoorLock>();
            if (doorLock != null && doorLock.IsLocked)
            {
                // Exit if the door is locked.
                return;
            }

            // If the door is not locked, start the transition to the new area.
            StartCoroutine(MovePlayerToDoor());
        }
    }

    public IEnumerator MovePlayerToDoor()
    {
        // Prevent other transitions from starting while this one is in progress.
        isTransitioning = true;

        if (audioManager != null)
        {
            // Play the door sound effect.
            audioManager.PlaySFX(audioManager.doorSound);
        }

        if (transitionAnimation != null)
        {
            // Trigger the transition animation to start.
            transitionAnimation.SetTrigger("Finish");
            yield return new WaitForSeconds(1f);
        }

        if (playerRb != null)
        {
            // Stop the player movement and set the Rigidbody2D to kinematic to prevent physics interactions during the transition.
            playerRb.velocity = Vector2.zero;
            playerRb.isKinematic = true;
        }

        // Move the player to the exit point.
        player.transform.position = exitPoint.position;

        // Update the camera's confiner to the new map boundary if it exists.
        if (confiner != null && newMapBoundary != null)
        {
            confiner.enabled = false;
            confiner.m_BoundingShape2D = newMapBoundary;
            confiner.InvalidatePathCache();
            confiner.enabled = true;
        }

        // Ensure the camera still follows the player movement.
        if (cinemachineCam != null)
        {
            cinemachineCam.Follow = player.transform;
        }

        // Reset the player's Rigidbody2D to allow movement again.
        if (playerRb != null)
        {
            playerRb.isKinematic = false;
        }

        // Trigger the transition animation to finish.
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger("Start");
        }

        // Allow other transitions to start again in the future.
        isTransitioning = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player has entered the trigger area of the door to allow interaction.
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // If the player exits the trigger area, set isPlayerNear to false to prevent interaction.
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    public void Interact()
    {
        if (!isTransitioning)
        {
            StartCoroutine(MovePlayerToDoor());
        }
    }

    public bool CanInteract()
    {
        return true;
    }
}
