using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Defines a ScriptableObject to hold all the data related to NPC dialogues.
[CreateAssetMenu(fileName="NewNPCDialogue", menuName="NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName; // Name of the NPC.
    public Sprite npcPortrait; // Portrait of the NPC.
    public string[] dialogueLines; // Array of dialogue lines.
    public bool[] autoProgress; // Array to determine if the dialogue should auto-progress.
    public bool[] endDialogue; // Array to determine if the dialogue ends after this line.
    public float autoProgressDelay = 1.5f; // Delay before auto-progressing to the next line.
    public float typingSpeed = 0.05f; // Speed of typing effect.
    public AudioClip typingSound; // Sound effect for typing.
    public float voicePitch = 1.0f; // Pitch of the voice sound effect.

    public DialogueChoice[] choices; // Array of dialogue choices.
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex; // Dialogue line where choices appear.
    public string[] choices; // Choices to display.
    public int[] nextDialogueIndex; // Indices of the next dialogue lines for each choice.
}