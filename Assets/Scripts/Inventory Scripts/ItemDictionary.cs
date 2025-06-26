using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores and manages all the available item prefabs in a dictionary, indexed by a unique ID.
//The ID is set in the Awake method, and the dictionary is populated with the item prefabs.
public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;//List of item prefabs to be stored in the dictionary.
    private Dictionary<int, GameObject> itemDictionary;//Dictionary to store item prefabs with their unique IDs.

    public void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        //Assign unique IDs to each item prefab in the list.
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i+1;
            }
        }

        //Populate the dictionary with item prefabs and their IDs.
        foreach (Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    //Method to get the item prefab by its unique ID.
    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);

        //If the item ID is not found in the dictionary, log an error message.
        if (prefab == null)
        {
            Debug.LogError("Item with ID " + itemID + " not found in the dictionary.");
        }

        return prefab;
    }
}
