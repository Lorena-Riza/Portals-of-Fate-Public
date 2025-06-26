using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Responsible for loading the game from a save file if there is any.
//If there is no save file, the continue button will be disabled.
public class ContinueButton : MonoBehaviour
{
    private string saveFilePath; //Path to the save file.
    [SerializeField] private GameObject menuUI; //Reference to the menu UI.
    [SerializeField] private Button continueButton; //Reference to the continue button.

    void Start()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        //Check if the save file exists and disable the continue button if it doesn't.
        if (!File.Exists(saveFilePath))
        {
            continueButton.interactable = false;
        }
    }

    public void LoadGame()
    {
        //Set the PlayerPrefs to load from save.
        PlayerPrefs.SetInt("LoadFromSave", 1);
        PlayerPrefs.Save();

        //Disable the menu UI if it is active.
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }

        //If the save file exists, load the game from the save file.
        if (File.Exists(saveFilePath))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveFilePath));
            if (!string.IsNullOrEmpty(saveData.sceneName))
            {
                StartCoroutine(PlayTransitionAndLoad(saveData.sceneName));
            }
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }

    //Plays the scene transition and loads the scene.
    private IEnumerator PlayTransitionAndLoad(string sceneToLoad)
    {
        if (SceneController.instance != null)
        {
            yield return SceneController.instance.PlayTransition();
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
