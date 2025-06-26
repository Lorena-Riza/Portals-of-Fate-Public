using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//This script manages the inventory system.
//It initializes the inventory slots and key bindings.
//It handles item selection and usage.
//Adds items to the first empty slot in the inventory.
//Highlights the selected item.
//Saves and loads inventory data.

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary; //Reference to the item dictionary, looks up item prefabs by ID.

    public GameObject inventoryPanel; //Reference to the inventory UI panel.
    public GameObject slotPrefab; //Prefab for the inventory slots.
    public int slotCount; //Number of slots in the inventory.
    public GameObject[] itemPrefabs; //Array of item prefabs to be used in the inventory.

    private List<Slot> slots = new List<Slot>(); //List of slots in the inventory.

    private Key[] inventoryKeys; //Array of keys for inventory navigation (1-0, -, =).
    private int currentlySelectedIndex = -1; //Index of the currently selected slot.

    private OpenItem selectedItemPanel; //UI panel for the selected item.
    public PlayerInput playerInput; //Reference to the PlayerInput component for handling input actions.
    public OpenItem[] openItemPanels; // Array of item panels to manage multiple openable items
    private Dictionary<string, OpenItem> itemNameToPanelMap = new Dictionary<string, OpenItem>(); // Maps itemName to corresponding OpenItem panel

    private void Awake()
    {
        //Find the item dictionary in the scene.
        itemDictionary = FindAnyObjectByType<ItemDictionary>();

        //Initialize the key bindings for inventory slots.
        inventoryKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            inventoryKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : i == 9 ? Key.Digit0 : i == 10 ? Key.Minus : Key.Equals;
        }

        // Build a map from item names to their OpenItem panels for quick lookup
        foreach (OpenItem panel in openItemPanels)
        {
            if (panel != null && panel.itemPrefab != null)
            {
                Item item = panel.itemPrefab.GetComponent<Item>();
                if (item != null && !itemNameToPanelMap.ContainsKey(item.itemName))
                {
                    itemNameToPanelMap.Add(item.itemName, panel);
                }
            }
        }

        // Safely register input event when interacting with an item from the inventory.
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed += OnInteract;
        }
        else
        {
            Debug.LogWarning("[InventoryController] PlayerInput reference is null in Awake.");
        }
    }

    //Unregister the input event when the object is disabled.
    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed -= OnInteract;
        }
    }

    //Function called when the interact action is performed.    
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log($"OnInteract called. InventoryPanel active: {inventoryPanel.activeSelf}, InventoryUIController.IsInventoryOpen: {InventoryUIController.IsInventoryOpen}, SelectedIndex: {currentlySelectedIndex}");

        if (currentlySelectedIndex < 0 || currentlySelectedIndex >= inventoryPanel.transform.childCount)
            return;

        if (!InventoryUIController.IsInventoryOpen)
        {
            Debug.Log("Inventory is not open, ignoring interact.");
            return;
        }

        //Get the currently selected slot.
        Slot slot = inventoryPanel.transform.GetChild(currentlySelectedIndex).GetComponent<Slot>();

        //Check if the slot has an item.
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null)
            {
                Debug.Log("Selected Item: " + item.itemName);

                if (itemNameToPanelMap.TryGetValue(item.itemName, out OpenItem panel))
                {
                    panel.ToggleItemPanelForItem(item.itemName);
                }
            }
        }
    }

    //Function called when the object is enabled.
    private void Update()
    {
        //Check if any inventory key is pressed this frame.
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[inventoryKeys[i]].wasPressedThisFrame)
            {
                SelectItem(i);//Select the item in the inventory.
                UseItemInSlot(i);//Use the item in the selected slot if it is available.
            }
        }
    }

    private void SelectItem(int index)
    {
        //Removes the Highlight of the previously selected slot.
        if (currentlySelectedIndex >= 0 && currentlySelectedIndex < inventoryPanel.transform.childCount)
        {
            Slot previousSlot = inventoryPanel.transform.GetChild(currentlySelectedIndex).GetComponent<Slot>();
            previousSlot.SetHighlight(false);
        }

        Slot slot = inventoryPanel.transform.GetChild(index).GetComponent<Slot>();

        if (slot.currentItem != null)
        {
            //Get the panel for the selected item.
            selectedItemPanel = slot.currentItem.GetComponent<OpenItem>();
        }
        else
        {
            selectedItemPanel = null;
        }

        ////Highlight the selected slot.
        slot.SetHighlight(true);
        currentlySelectedIndex = index;
    }

    //This function is called when the item is used if there is an item in the selected slot.
    private void UseItemInSlot(int index)
    {
        Slot slot = inventoryPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            // Track item used
            AnalyticsManager.Instance.TrackItemUsed(
                item.itemName,
                "general_use" // or specific context
            );
            item.UseItem();
        }
    }

    //Adds an item to the first empty slot in the inventory.
    public void AddItemToFirstEmptySlot(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            Item item = itemPrefab.GetComponent<Item>();
            if (item != null)
            {
                // Track item collected
                AnalyticsManager.Instance.TrackItemCollected(
                    item.itemName,
                    "puzzle_reward"
                );
            }
        }

        bool itemAdded = false;
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem == null)
            {
                //Instantiate the item prefab and set its parent to the slot.
                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
                itemAdded = true;
                break;
            }
        }

        if (itemAdded)
        {
            Debug.Log("Item successfully added to inventory.");
        }
        else
        {
            Debug.LogWarning("No empty slot found in inventory.");
        }
    }

    public bool RemoveItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                // Check if the currentItem’s prefab matches the itemPrefab to remove
                if (slot.currentItem.name.StartsWith(itemPrefab.name))
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                    Debug.Log($"Removed item {itemPrefab.name} from inventory.");
                    return true;
                }
            }
        }
        Debug.LogWarning($"Item {itemPrefab.name} not found in inventory.");
        return false;
    }

    //This function is called to save the inventory data.
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        //Iterate through each slot in the inventory panel.
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                //Get the item component from the current item in the slot.
                Item item = slot.currentItem.GetComponent<Item>();
                //Add the item ID and slot index to the inventory data list.
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex()
                });
            }
        }
        return invData;
    }

    //Loads the inventory items from the saved data.
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        //Clears the current inventory panel.
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Instantiate the specified number of slots in the inventory panel.
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        //Populates the inventory slots with items based on the saved data.
        if (inventorySaveData != null)
        {
            foreach (InventorySaveData data in inventorySaveData)
            {
                if (data.slotIndex < slotCount)
                {
                    Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                    GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                    if (itemPrefab != null)
                    {
                        GameObject item = Instantiate(itemPrefab, slot.transform);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        slot.currentItem = item;
                    }
                }
            }
        }
    }

    public bool HasItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                if (slot.currentItem.name.StartsWith(itemPrefab.name))
                {
                    return true;
                }
            }
        }
        return false;
    }
}