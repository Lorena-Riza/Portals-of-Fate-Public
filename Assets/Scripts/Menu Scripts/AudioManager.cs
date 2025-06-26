using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the audio for the game, including background music and sound effects.
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource; //The audio source for background music.
    public AudioSource sfxSource; //The audio source for sound effects.

    //Background music and sound effect clips.
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;
    public AudioClip footstepSound;
    public AudioClip jumpSound;
    public AudioClip chestSound;
    public AudioClip doorSound;
    public AudioClip bellSound1;
    public AudioClip bellSound2;
    public AudioClip bellSound3;
    public AudioClip voice1;
    public AudioClip voice2;
    public AudioClip voice3;
    public AudioClip voice4;
    public AudioClip voice5;

    private void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    //Methos to play a sound effect.
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
