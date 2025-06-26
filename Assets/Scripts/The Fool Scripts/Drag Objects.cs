using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Implements the drag and drop functionality for UI elemnts.
public class DragObjects : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // Reference to the RectTransform component of the object.
    private CanvasGroup canvasGroup; // Reference to the CanvasGroup component of the object.
    private Canvas canvas; // Reference to the parent Canvas component.
    private Vector2 originalPosition; // The original position of the object.

    private void Awake()
    {
        // Get the references of the required components.
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        // Store the original position of the object.
        originalPosition = rectTransform.anchoredPosition;
    }

    // Called when draggin begins.
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0.6f; // Make the object semi-transparent.
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false; // Disable raycasting to allow the object to be dragged over other UI elements.
    }

    // Called during dragging.
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Update the position of the object based on the pointer;s delta movement, adjusted to the canvas scale.
    }
    
    // Called when dragging ends.
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f; // Restores the full visibility of the object.
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true; // Re-enable raycasting to allow the object to interact with other UI elements.
    }
}
