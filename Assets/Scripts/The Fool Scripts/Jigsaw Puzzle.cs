using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls a drag-and-drop jigsaw puzzle with reward logic and inventory integration.
public class JigsawPuzzle : MonoBehaviour
{
    [Header("Puzzle Setup")]
    public GameObject puzzlePiecePrefab; // Prefab for puzzle pieces.
    public Sprite[] puzzleSprites; // Assign sprites here in correct order.
    public Transform spawnArea;    // UI panel where pieces are scattered.
    public Transform targetGrid;   // PuzzleGrid (where pieces should be dropped).

    [Header("Puzzle Info")]
    [SerializeField] private int myPuzzleIndex;             // Puzzle index for PuzzleController.
    [SerializeField] private InventoryController inventoryController; // Reference to InventoryController.
    [SerializeField] private GameObject rewardItemPrefab;   // Item prefab to add after completion.
    [SerializeField] private GameObject itemToRemovePrefab; // Item prefab to remove.

    private float puzzleStartTime; // Time when the puzzle was started.

    // Called when the scene starts.
    private void Start()
    {
        puzzleStartTime = Time.unscaledTime;
        AnalyticsManager.Instance.TrackPuzzleAttempt(myPuzzleIndex, "jigsaw");
        SpawnPuzzlePieces(); // Generate puzzle pieces and place them randomly in the spawn area.
    }

    private void Update()
    {
        // For testing: Press Space to check if the puzzle is solved.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed – calling CheckWin()");
            CheckWin();
        }
    }

    // Spawns all the puzzle pieces in random order into the spawn area.
    public void SpawnPuzzlePieces()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < puzzleSprites.Length; i++)
            indices.Add(i);

        Shuffle(indices);

        RectTransform panelRect = spawnArea.GetComponent<RectTransform>();
        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        for (int i = 0; i < puzzleSprites.Length; i++)
        {
            GameObject piece = Instantiate(puzzlePiecePrefab, spawnArea);
            piece.name = "Piece_" + indices[i];
            piece.GetComponent<Image>().sprite = puzzleSprites[indices[i]];

            RectTransform pieceRect = piece.GetComponent<RectTransform>();

            // Ensures the piece is not controlled by any layout system
            LayoutElement layout = piece.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.ignoreLayout = true;
            }

            // Sets the pivot to center to simplify positioning
            pieceRect.pivot = new Vector2(0.5f, 0.5f);

            // Random anchored position in centered coordinates
            float x = Random.Range(-panelWidth / 2f, panelWidth / 2f);
            float y = Random.Range(-panelHeight / 2f, panelHeight / 2f);
            pieceRect.anchoredPosition = new Vector2(x, y);
        }
    }

    // Fisher-Yates Shuffle to randomize list contents.
    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // Called to verify whether the player has correctly arranged all puzzle pieces.
    public void CheckWin()
    {
        Debug.Log("CheckWin() called");
        Debug.Log($"targetGrid: {targetGrid.name}, childCount: {targetGrid.childCount}");

        if (targetGrid.childCount == 0)
        {
            Debug.LogError("targetGrid has no children!");
            return;
        }

        // First child of targetGrid is expected to hold the puzzle slots.
        Transform slotsContainer = targetGrid.GetChild(0);
        Debug.Log($"Slots container: {slotsContainer.name}, childCount: {slotsContainer.childCount}");

        // Safety check: ensure expected number of slots.
        if (slotsContainer.childCount < puzzleSprites.Length)
        {
            Debug.LogError($"Expected at least 16 slots inside {slotsContainer.name}, but found {slotsContainer.childCount}");
            return;
        }

        // Loop through each expected slot index based on the number of sprites.
        for (int i = 0; i < puzzleSprites.Length; i++)
        {
            Transform slot = slotsContainer.GetChild(i);
            Debug.Log($"Slot {i} ({slot.name}), childCount: {slot.childCount}");

            if (slot.childCount == 0)
            {
                Debug.Log($"Puzzle Slot {i} has no children.");
                return;
            }

            // Look for the active puzzle piece in the slot.
            Transform piece = null;
            for (int c = 0; c < slot.childCount; c++)
            {
                if (slot.GetChild(c).gameObject.activeSelf)
                {
                    piece = slot.GetChild(c);
                    break;
                }
            }

            if (piece == null)
            {
                Debug.Log($"Puzzle Slot {i} has no active puzzle piece.");
                return;
            }

            // Check if the piece name matches the expected name.
            string expectedName = $"Piece_{i}";
            string actualName = piece.name;
            Debug.Log($"Slot {i}: expected {expectedName}, got {actualName}");

            if (actualName != expectedName)
            {
                Debug.Log("Not solved yet");
                return; // Early exit if any piece is incorrect.
            }
        }

        Debug.Log("Puzzle Solved!");
        OnPuzzleCompleted(); // If all checks pass, trigger completion logic.
    }

    // Called once when the puzzle is completed successfully.
    private void OnPuzzleCompleted()
    {
        float timeTaken = Time.unscaledTime - puzzleStartTime;
        AnalyticsManager.Instance.TrackPuzzleComplete(
            myPuzzleIndex,
            "jigsaw",
            timeTaken
        );

        // Mark the puzzle as completed in the PuzzleController.
        PuzzleController.Instance.CompletePuzzle(myPuzzleIndex);
        PuzzleController.Instance.HidePuzzle();

        // Remove the required item from inventory, if any.
        if (itemToRemovePrefab != null)
        {
            inventoryController.RemoveItem(itemToRemovePrefab);
            Debug.Log("Item removed from inventory.");
        }
        else
        {
            Debug.LogWarning("Item to remove prefab is null.");
        }

        // Add the reward item to the inventory.
        if (rewardItemPrefab != null)
        {
            Item rewardItem = rewardItemPrefab.GetComponent<Item>();
            if (rewardItem != null)
            {
                rewardItem.PickUp();
                inventoryController.AddItemToFirstEmptySlot(rewardItemPrefab);
                Debug.Log("Reward item added to inventory.");
            }
            else
            {
                Debug.LogWarning("Reward item prefab has no Item component.");
            }
        }
        //Disable the puzzle UI.
        gameObject.SetActive(false);
    }
}