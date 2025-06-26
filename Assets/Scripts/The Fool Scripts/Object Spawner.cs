using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is responsible for spawning draggable objects as well as a special interactable item.
public class ObjectSpawner : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private Sprite[] objectAssets; // The array of sprites to be used for the draggable objects.
    [Header("Settings")]
    [SerializeField] private GameObject draggableObjectPrefab; // The prefab for the draggable objects.
    [SerializeField] private GameObject specialItemPrefab; // The prefab for the special item.
    [SerializeField] private RectTransform panel; // The panel where the objects will be spawned.
    [SerializeField, Min(1)] private int numberOfObjects = 5; // The number of draggable objects to spawn.

    [Header("Inventory Settings")]
    [SerializeField] private InventoryController inventoryController; // A reference to the inventory controller.
    [SerializeField] private GameObject inventoryItemPrefab; // The prefab for the inventory item to be added.

    [SerializeField] private int myPuzzleIndex; // The index of the puzzle associated with this spawner so it can be marked as completed.

    private float puzzleStartTime;  // The time when the puzzle was started.
    private void Start()
    {
        puzzleStartTime = Time.unscaledTime;
        AnalyticsManager.Instance.TrackPuzzleAttempt(myPuzzleIndex, "find_item");

        // Spawn the objects and the special item when the script starts.
        SpawnObjects(numberOfObjects);
        SpawnSpecialItem();
    }

    // Spawns a specified number of draggable objects with random positions and random sprites.
    public void SpawnObjects(int numberOfObjects)
    {
        // Clear any existing objects in the panel before spawning new ones.
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate and position each object.
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Crate a new draggable object.
            GameObject draggableObject = Instantiate(draggableObjectPrefab, panel);
            RectTransform rectTransform = draggableObject.GetComponent<RectTransform>();

            // Chooses a random sprite from the objectAssets array and assigns it to the draggable object.
            int randomAssetIndex = Random.Range(0, objectAssets.Length);
            Image objectImage = draggableObject.GetComponent<Image>();
            objectImage.sprite = objectAssets[randomAssetIndex];

            // Sets the random position for the draggable object within the panel's bounds.
            float randomX = Random.Range(-panel.rect.width / 2f, panel.rect.width / 2f);
            float randomY = Random.Range(-panel.rect.height / 2f, panel.rect.height / 2f);
            rectTransform.anchoredPosition = new Vector2(randomX, randomY);
        }
    }

    // Spawns the special item which can be clicked complete the puzzle.
    private void SpawnSpecialItem()
    {
        GameObject specialItem = Instantiate(specialItemPrefab, panel); // Crates a new special item.
        specialItem.transform.SetAsFirstSibling(); // Send it to the back of the panel.
        RectTransform rectTransform = specialItem.GetComponent<RectTransform>();

        // Sets a random position for the special item within the panel's bounds.
        rectTransform.anchoredPosition = new Vector2(Random.Range(-panel.rect.width / 2f, panel.rect.width / 2f),
                                                      Random.Range(-panel.rect.height / 2f, panel.rect.height / 2f));

        // Adds a click event handler to the special item.
        Button itemButton = specialItem.GetComponent<Button>();

        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnSpecialItemClicked);
        }
        else
        {
            Debug.LogError("No Button component found on the special item prefab!");
        }
    }

    // This method is called when the special item is clicked.
    public void OnSpecialItemClicked()
    {
        float timeTaken = Time.unscaledTime - puzzleStartTime;
        AnalyticsManager.Instance.TrackPuzzleComplete(
            myPuzzleIndex,
            "find_item",
            timeTaken
        );

        Item item = inventoryItemPrefab.GetComponent<Item>(); // Get the Item component from the prefab.
        // Adds the item to the inventory if the inventory controller is not null.
        if (inventoryController != null)
        {
            item.PickUp(); // Call the PickUp method on the item.
            inventoryController.AddItemToFirstEmptySlot(inventoryItemPrefab);
        }

        // Hides the puzzle UI and sets the puzzle as being complete.
        PuzzleController.Instance.HidePuzzle();
        PuzzleController.Instance.CompletePuzzle(myPuzzleIndex);
    }
}
