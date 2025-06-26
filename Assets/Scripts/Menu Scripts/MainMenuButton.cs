using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles the transition between the current scene and the main menu when the button is pressed.
public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject menuUI; //Reference to the menu UI GameObject.

    //This method is called when the button is pressed.
    public void GoToMainMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(false);
        }

        SceneController.instance.LoadSceneWithTransition("Main Menu");
    }
}
