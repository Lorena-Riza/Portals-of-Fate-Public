using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Plays the sound effect when one of the menu buttons is clicked.
public class ButtonSound : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void StartSound()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
    }
}
