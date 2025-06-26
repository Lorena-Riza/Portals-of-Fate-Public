using UnityEngine;

// Manages the display of a UI panel for a specific in game item.
// It only shows the item if the player interacts with the correct item.
public class OpenItem : MonoBehaviour
{
    public GameObject itemPanel; // The UI panel to show/hide.
    public GameObject itemPrefab; // The prefab of the item to check against. 

    private string targetItemName; // The name of the targeted item.

    private void Awake()
    {
        // Initialize the target item name based on the item prefab.
        if (itemPrefab != null)
        {
            Item item = itemPrefab.GetComponent<Item>();
            if (item != null)
            {
                targetItemName = item.itemName;
            }
        }

        // Makes sure the item panel starts hidden.
        if (itemPanel != null)
        {
            itemPanel.SetActive(false);
        }
    }

    // This method is called to toggle the visibility of the item panel when the player interacts with it.
    public void ToggleItemPanelForItem(string itemName)
    {
        // If the panel is not assigned, do nothing.
        if (itemPanel == null) return;

        // If the panel is assigned, check if the item name matches the target item name.
        if (itemName == targetItemName)
        {
            bool isActive = itemPanel.activeSelf;
            itemPanel.SetActive(!isActive);
        }
        else
        {
            // If the item name does not match, ensure the panel is hidden.
            itemPanel.SetActive(false);
        }
    }
}