using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles the transition between scenes
public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private float transitionDuration = 1f;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of SceneController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //Persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public IEnumerator PlayTransition()
    {
        if (transitionAnimation != null)
        {
            transitionAnimation.SetTrigger("Finish");// Trigger the animation to start the transition
            yield return new WaitForSeconds(transitionDuration);
            transitionAnimation.SetTrigger("Start"); // Trigger the animation to end the transition
        }
        else
        {
            Debug.LogWarning("Transition animation is missing!");
        }
    }
    
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    //Delay the scene loading until the transition animation is finished
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return PlayTransition();
        SceneManager.LoadScene(sceneName);
    }
}