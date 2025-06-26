using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Game is quitting..."); // Logs message in the editor
        Application.Quit(); // Quits the game (won't work in the editor)
    }
}
