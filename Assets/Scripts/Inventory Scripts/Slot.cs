using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is attached to each slot in the inventory UI.
public class Slot : MonoBehaviour
{
    public GameObject currentItem; //Reference to the current item in the slot.
    public Image highlightImage; //Image used to indicate the slot is highlighted.

    private void Start()
    {
        //Initialize the slot with no item and highlight image disabled.
        highlightImage.enabled = false;
    }

    //Enables or disables the highlight image based on the user selection.
    public void SetHighlight(bool active)
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = active;
            Debug.Log($"Highlight {(active ? "ON" : "OFF")} for {gameObject.name}");
        }
    }
}
