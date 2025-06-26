using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Manages the dialogue UI and interactions with the player.
public class DialogueController : MonoBehaviour
{
    // Singleton instance of the DialogueController.
    public static DialogueController Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialogueBox; // The dialogue box UI element.
    public TMP_Text dialogueText; // The text component for displaying dialogue.
    public TMP_Text nameText; // The text component for displaying the NPC's name.
    public Image portraitImage; // The image component for displaying the NPC's portrait.
    public Transform choiceContainer; // The container for holding choice buttons.
    public GameObject choiceButtonPrefab; // The prefab for choice buttons.

    //Ensure that only one instance of DialogueController exists.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDialogueUI(bool show)
    {
        dialogueBox.SetActive(show); // Toggle the visibility of the dialogue box.
    }

    public void SetNPCInfo (string npcName, Sprite npcPortrait)
    {
        nameText.text = npcName; // Set the NPC's name in the dialogue box.
        portraitImage.sprite = npcPortrait; // Set the NPC's portrait in the dialogue box.
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text; // Set the dialogue text in the dialogue box.
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject); // Destroy all existing choice buttons.
        }
    }

    public GameObject CreateChoiceButton (string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText; // Set the text of the choice button.
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick); // Add the onClick listener to the button.
        return choiceButton; // Return the created choice button.
    }
}
