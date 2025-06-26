using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Holds the item data and checks if the item is used.
public class Item : MonoBehaviour
{
    public int ID;
    public string itemName;

    public virtual void UseItem()
    {
        Debug.Log($"Using item: {itemName}");
    }

    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickup(itemName, itemIcon);
        }
        else
        {
            Debug.LogWarning("ItemPickUpUIController instance is null. Cannot show item pickup UI.");
        }
    }
}
