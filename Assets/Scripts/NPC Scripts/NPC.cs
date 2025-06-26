using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// This script handles the interaction the player has with an NPC.
// It manages line progression, typing effects, auto-progressing lines, dialogue branching and interaction with other systems.
public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")] 
    public NPCDialogue dialogueData; // The dialogue data for this NPC.
    private DialogueController dialogueUI; // The UI controller for displaying dialogue.

    private int dialogueIndex; // The current index of the dialogue line being displayed.
    private bool isTyping; // Indicates if the current line is being typed out.
    private static bool isDialogueActive = false; // Only one dialogue at a time.
    public static bool IsDialogueActive => isDialogueActive; // Public read-only access.

    private PlayerInput playerInput; // Reference to the PlayerInput component.
    private AudioManager audioManager; // Reference to the AudioManager for playing sounds.
    private Coroutine typingCoroutine; // Coroutine for typing effect.

    private bool playerInRange = false; // Indicates if the player is in range to interact with the NPC.

    private void Start()
    {
        // Initialize the dialogue UI controller.
        dialogueUI = DialogueController.Instance;
    }

    private void Awake()
    {
        // Find the PlayerInput in the scene.
        playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("[NPC] PlayerInput not found in scene.");
        }

        // Find the AudioManager in the scene.
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogWarning("[NPC] AudioManager not found in scene.");
        }
    }

    // This method is called when the script is enabled.
    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed += OnInteractInput;
        }
    }

    // This method is called when the script is disabled.
    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed -= OnInteractInput;
        }
    }

    // This method is called when the object is destroyed.
    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Interact"].performed -= OnInteractInput;
        }
    }

    // This method is called when the interact input is performed.
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        Debug.Log($"[NPC] Interact input received from {gameObject.name}, dialogueActive={isDialogueActive}");

        if ((CanInteract() || isDialogueActive) && playerInRange)
        {
            Interact();
        }
    }

    // This method checks if the player can interact with the NPC.
    public bool CanInteract()
    {
        return !isDialogueActive && !MenuController.IsMenuOpen && !InventoryUIController.IsInventoryOpen;
    }

    // This method is called when the player interacts with the NPC.
    public void Interact()
    {
        // Check if the NPC has dialogue data assigned.
        if (dialogueData == null)
        {
            Debug.LogWarning($"[NPC] No dialogue data assigned to {gameObject.name}");
            return;
        }

        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else
        {
            NextLine();
        }
    }

    // Begins the dialogue with the NPC.
    private void StartDialogue()
    {
        // Track dialogue start
        AnalyticsManager.Instance.TrackDialogueEvent(
        dialogueData.npcName,
        dialogueIndex
         );

        isDialogueActive = true;
        isTyping = false;

        dialogueUI.nameText?.SetText(dialogueData.npcName);
        dialogueUI.portraitImage.sprite = dialogueData.npcPortrait;
        dialogueUI.dialogueBox.SetActive(true);

        // Start the dialogue from a specific index based on the player's inventory or other conditions.
        ChestPuzzle chestPuzzle = GetComponent<ChestPuzzle>();

        if (chestPuzzle != null)
        {
            int dialogueStartIndex = chestPuzzle.GetDialogueIndexBasedOnInventory();

            if (dialogueStartIndex != -1)
            {
                dialogueIndex = dialogueStartIndex;
                typingCoroutine = StartCoroutine(TypeLine());
                return;
            }
        }

        // Fallback if no special dialogue line is needed
        HintGiver hintGiver = GetComponent<HintGiver>();
        if (hintGiver != null && HintGiver.HintGiven)
        {
            dialogueIndex = hintGiver.GetEndLine();
        }
        else
        {
            dialogueIndex = 0;
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    // Goes to the next line of dialogue or ends the dialogue if there are no more lines.
    private void NextLine()
    {
        // If text is currently being typed, stop the typing coroutine and display the full line.
        if (isTyping)
        {
            StopCoroutine(typingCoroutine); // Stop the typing effect.
            dialogueUI.dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]); // Show the full line.
            isTyping = false;
        }

        // Clear any existing choices from the UI.
        dialogueUI.ClearChoices();

        // Check if the current line is an end line.
        if (dialogueData.endDialogue.Length > dialogueIndex && dialogueData.endDialogue[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        // Look for choices configured for the current line.
        foreach (DialogueChoice choice in dialogueData.choices)
        {
            if (choice.dialogueIndex == dialogueIndex)
            {
                // Handle the case where hints are given and skip the choice if the hint has already been given.
                HintGiver hintGiver = GetComponent<HintGiver>();
                if (hintGiver != null && HintGiver.HintGiven && hintGiver.IsHintChoiceIndex(dialogueIndex))
                {
                    dialogueIndex = hintGiver.GetEndLine(); // Skip to the end line.
                    typingCoroutine = StartCoroutine(TypeLine()); //Continue typing the next line.
                    return;
                }

                // Display the choices to the player.
                DisplayChoices(choice);
                return;
            }
        }

        // If no choices are found, continue to the next line.
        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // Display the next line of dialogue.
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            // If there are no more lines, end the dialogue.
            EndDialogue();
        }
    }

    // Typing effect coroutine for the current line of dialogue.
    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.dialogueText.SetText(""); // Clear the text before typing the new line.

        string line = dialogueData.dialogueLines[dialogueIndex];
        Debug.Log($"[NPC] Typing line {dialogueIndex}: {line}");

        // Type each character one at a time.
        foreach (char letter in line)
        {
            dialogueUI.dialogueText.text += letter;

            // Play typing sound if configured.
            if (audioManager != null && dialogueData.typingSound != null)
            {
                if (!audioManager.sfxSource.isPlaying)
                {
                    audioManager.sfxSource.pitch = dialogueData.voicePitch;
                    audioManager.sfxSource.clip = dialogueData.typingSound;
                    audioManager.sfxSource.Play();
                }
            }

            // Wait for the specified typing speed before typing the next character.
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Auto-progress to the next line if configured.
        if (dialogueData.autoProgress.Length > dialogueIndex && dialogueData.autoProgress[dialogueIndex])
        {
            Debug.Log($"[NPC] Auto-progressing to next line in {dialogueData.autoProgressDelay} seconds.");
            // Wait for the specified delay before auto-progressing.
            yield return new WaitForSecondsRealtime(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    // Displays the choices available for the current line of dialogue.
    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            // Target line index if the option is selected.
            int nextIndex = choice.nextDialogueIndex[i];
            // Create a button for each choice.
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex)); 
        }
    }

    // Handles the player's choice selection.
    void ChooseOption(int nextIndex)
    {
        Debug.Log($"[NPC] ChooseOption called with nextIndex={nextIndex}, current dialogueIndex={dialogueIndex}");

        ChestPuzzle chestPuzzle = GetComponent<ChestPuzzle>();

        // Check if the NPC has a ChestPuzzle component.
        if (chestPuzzle != null)
        {
            Debug.Log("[NPC] ChestPuzzle component found.");
            // Check if the current dialogue index is a choice index.
            if (chestPuzzle.IsChoiceDialogueIndex(dialogueIndex))
            {
                Debug.Log("[NPC] At choice dialogue index, calling LogChoice.");
                chestPuzzle.LogChoice(nextIndex);
            }
            else
            {
                Debug.Log("[NPC] Not at choice dialogue index.");
            }
        }
        else
        {
            Debug.Log("[NPC] No ChestPuzzle component found.");
        }

        // Mark the hint as given if applicable.
        HintGiver hintGiver = GetComponent<HintGiver>();
        if (hintGiver != null)
        {
            hintGiver.MarkHintGiven(nextIndex); // Mark the hint as given.
        }

        dialogueIndex = nextIndex; // Update the dialogue index to the next line.
        dialogueUI.ClearChoices(); // Clear the choices from the UI.
        DisplayCurrentLine(); // Display the current line of dialogue.
    }

    // Displays the current line of dialogue.
    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    // Ends the dialogue with the NPC.
    public void EndDialogue()
    {
        // Check if the dialogue is already inactive.
        if (!isDialogueActive) return;

        Debug.Log($"[NPC] Ending dialogue with {dialogueData.npcName}");

        StopAllCoroutines(); // Stop any ongoing typing effects.
        isDialogueActive = false;
        isTyping = false;

        // Reset pitch to default
        if (audioManager != null)
        {
            audioManager.sfxSource.pitch = 1.0f;
        }

        // Clear the dialogue UI.
        if (dialogueUI.dialogueText != null) dialogueUI.dialogueText.SetText("");
        if (dialogueUI.dialogueBox != null) dialogueUI.dialogueBox.SetActive(false);
    }

    // This method is called when the player enters the trigger collider of the NPC.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detects when the player exits the trigger collider of the NPC.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}