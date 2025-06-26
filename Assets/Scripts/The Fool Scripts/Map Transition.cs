using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary; // Reference to the boundary collider for the new map area.
    [SerializeField] private Animator transitionAnimation; // Reference to the animator for the transition animation.
    [SerializeField] private float transitionCooldown = 1.5f; // Cooldown time before the next transition can occur.

    private CinemachineConfiner confiner; // Reference to the CinemachineConfiner component.
    private static bool isTransitioning = false; // Static variable to prevent multiple transitions at once.

    /// Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Automatically find the CinemachineConfiner in the scene.
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Trigger transition only if the player enters and no transition is already running.
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(PlayTransition());
        }
    }

    // Coroutine that handles the full transition sequence.
    private IEnumerator PlayTransition()
    {
        isTransitioning = true; //Block other transitions.

        // Play "Finish" animation.
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger("Finish");
            yield return new WaitForSeconds(1f); // Wait for the animation to finish before proceeding.
        }

        // Switch the camera confiner to the new map's boundary.
        confiner.m_BoundingShape2D = mapBoundary;
        // Important to refresh the confiner path after changing boundaries.
        confiner.InvalidatePathCache();

        // Play "Start" animation.
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger("Start");
        }

        // Wait for cooldown to finish before allowing another transition.
        yield return new WaitForSeconds(transitionCooldown);

        isTransitioning = false; // Allow transitions again.
    }
}
