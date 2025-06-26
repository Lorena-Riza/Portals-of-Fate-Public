using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

//Chekcs for save file and asks for confirmation before starting
//a new game before overwriting the old one.
public class NewGameButton : MonoBehaviour
{
    //UI references set in the inspector
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject confirmNewGameCanvas;
    [SerializeField] private GameObject mainMenuCanvas;

    private string saveFilePath;

    private void Start()
    {
        //Initialize the save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        //Hide the confirmation canvas at the start
        if (confirmNewGameCanvas != null)
            confirmNewGameCanvas.SetActive(false);
    }

    public void StartNewGame()
    {
        //Check if the save file exists and prompt for confirmation
        if (File.Exists(saveFilePath))
        {
            
            if (confirmNewGameCanvas != null)
                confirmNewGameCanvas.SetActive(true);

            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
        }
        else
        {
            //No save exists so we can start a new game immediately
            BeginNewGame();
        }
    }

    public void ConfirmStartNewGame()
    {
        //If the confirmation canvas is active, hide it
        if (confirmNewGameCanvas != null)
            confirmNewGameCanvas.SetActive(false);

        BeginNewGame();
    }

    private void BeginNewGame()
    {
        Debug.Log("Starting New Game...");
        //Ensure the game is not paused
        Time.timeScale = 1;

        //Tells the game to load from the beginning
        PlayerPrefs.SetInt("LoadFromSave", 0);
        PlayerPrefs.Save();

        //Hide the main menu UI
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Menu UI is not assigned!");
        }

        //Transition to the game scene
        StartCoroutine(PlayTransitionAndLoad());
    }

    ///Method to play the transition and load the game scene
    private IEnumerator PlayTransitionAndLoad()
    {
        if (SceneController.instance != null)
        {
            yield return SceneController.instance.PlayTransition();
        }

        SceneManager.LoadScene("TheFool");
    }
}
