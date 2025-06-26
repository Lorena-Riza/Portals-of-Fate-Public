using Cinemachine;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Handles saving and loading game data, including player position, inventory, scenes and puzzle states.
// Automatically loads saved data on start or starts a new game based on the specified value in PlayerPrefs.
public class SaveController : MonoBehaviour
{
    private string saveFilePath; // Path to save file.
    private InventoryController inventoryController; // Reference to the InventoryController for managing inventory items.

    void Start()
    {
        Debug.Log("SaveController: Start");

        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json"); // Define the path for the save file.
        inventoryController = FindObjectOfType<InventoryController>(); // Find the InventoryController in the scene.

        if (inventoryController == null)
        {
            Debug.LogError("SaveController: InventoryController not found in scene!");
        }

        // Check whether to load from saved data or start a new game.
        int loadFromSave = PlayerPrefs.GetInt("LoadFromSave", 0);
        Debug.Log($"SaveController: LoadFromSave = {loadFromSave}");

        // Load game data if the flag is set and the save file exists.
        if (loadFromSave == 1 && File.Exists(saveFilePath))
        {
            Debug.Log("SaveController: Loading game from file...");
            LoadGame();
        }
        else
        {
            // If no save file exists or the flag is not set, create a new inventory.
            Debug.Log("SaveController: No save found or not instructed to load. Creating new inventory.");
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            SaveGame();
        }
    }

    // Saves the current game state to a JSON file.
    public void SaveGame()
    {
        Debug.Log("SaveController: Saving game...");

        // Creates a new SaveData object to store the current game state.
        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            sceneName = SceneManager.GetActiveScene().name,
            puzzleCompletionStates = PuzzleController.Instance.GetPuzzleCompletionStates(),
            hintGiven = HintGiver.HintGiven,
            doorLockStates = DoorController.Instance.GetDoorLockStates()
        };

        // Convert the SaveData object to JSON and write it to the save file.
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(saveData));
        Debug.Log("SaveController: Game saved successfully.");
    }

    // Loads the game state from a JSON file.
    public void LoadGame()
    {
        Debug.Log("SaveController: LoadGame() called");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("SaveController: Save file not found.");
            return;
        }

        // Read the JSON data from the save file and deserialize it into a SaveData object.
        string json = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // Restore player position. 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerPosition;
            Debug.Log("SaveController: Player position restored.");
        }
        else
        {
            Debug.LogError("SaveController: Player not found in scene!");
        }

        // Restore the map boundary.
        GameObject boundary = GameObject.Find(saveData.mapBoundary);
        if (boundary != null)
        {
            FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D =
                boundary.GetComponent<PolygonCollider2D>();
            Debug.Log("SaveController: Map boundary set.");
        }
        else
        {
            Debug.LogError($"SaveController: Map boundary '{saveData.mapBoundary}' not found!");
        }

        // Restore the inventory items.
        if (inventoryController != null)
        {
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            Debug.Log($"SaveController: Loaded {saveData.inventorySaveData.Count} inventory items.");
        }

        // Restore the puzzle completion states.
        if (saveData.puzzleCompletionStates != null)
        {
            PuzzleController.Instance.SetPuzzleCompletionStates(saveData.puzzleCompletionStates);
            Debug.Log("SaveController: Puzzle completion states restored.");
        }

        // Restore hint state.
        if (saveData.hintGiven)
        {
            HintGiver.ResetHintGiven();
            typeof(HintGiver)
                .GetField("hintGivenThisLevel", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, true);
            Debug.Log("SaveController: Hint state restored.");
        }

        // Restore the door lock states.
        if (saveData.doorLockStates != null)
        {
            DoorController.Instance.SetDoorLockStates(saveData.doorLockStates);
            Debug.Log("SaveController: Door lock states restored.");
        }
    }
}