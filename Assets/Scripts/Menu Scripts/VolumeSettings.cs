using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//Handles the volume settings for sound, music, and SFX.
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; //Reference to the AudioMixer.
    [SerializeField] private Slider soundSlider; //Reference to the sound volume slider (all sounds).
    [SerializeField] private Slider musicSlider; //Reference to the music volume slider (background music).
    [SerializeField] private Slider SFXSlider; //Reference to the SFX volume slider (sound effects).

    //File path for saving and loading volume settings.
    private string settingsFilePath;

    private void Start()
    {
        //Set the file path for saving and loading volume settings.
        settingsFilePath = Path.Combine(Application.persistentDataPath, "volumeSettings.json");

        //Load volume settings if the file exists, otherwise set default values and save them.
        if (File.Exists(settingsFilePath))
        {
            LoadVolume();
        }
        else
        {
            SetSoundVolume();
            SetMusicVolume();
            SetSFXVolume();
            SaveVolume();
        }

        //Add listeners to the sliders to update the volume when changed.
        soundSlider.onValueChanged.AddListener(delegate { SetSoundVolume(); SaveVolume(); });
        musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); SaveVolume(); });
        SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); SaveVolume(); });
    }

    //Set the volume for the general sound.
    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
    }
    //Set the volume for the music.
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    //Set the volume for the sound effects (SFX).
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
    //Class to hold volume data for saving and loading.
    private void SaveVolume()
    {
        VolumeData data = new VolumeData
        {
            soundVolume = soundSlider.value,
            musicVolume = musicSlider.value,
            sfxVolume = SFXSlider.value
        };

        File.WriteAllText(settingsFilePath, JsonUtility.ToJson(data));
    }
    //Load the volume settings from the JSON file.
    private void LoadVolume()
    {
        string json = File.ReadAllText(settingsFilePath);
        VolumeData data = JsonUtility.FromJson<VolumeData>(json);

        soundSlider.value = data.soundVolume;
        musicSlider.value = data.musicVolume;
        SFXSlider.value = data.sfxVolume;

        SetSoundVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}