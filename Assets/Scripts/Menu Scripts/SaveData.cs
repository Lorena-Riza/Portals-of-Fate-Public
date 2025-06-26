using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to save the player's position, inventory, and other data to a file
[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
    public string sceneName;
    public List<bool> puzzleCompletionStates;
    public List<bool> doorLockStates;
    public bool hintGiven;
}
