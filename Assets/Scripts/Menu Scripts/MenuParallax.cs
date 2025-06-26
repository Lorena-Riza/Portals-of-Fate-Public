using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//Adds a parallax effect to the menu background based on mouse movement
public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 1f; //How much the background moves in relation to the mouse movement.
    public float smoothTime = 3f; //How smooth the movement is. The higher the value, the smoother the movement.

    private Vector2 startPosition; //Stores the starting position of the background.
    private Vector3 velocity; //Velocity of the background movement.

    // Start is called before the first frame update
    void Start()
    {
        //Saves the starting position of the background.
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Ensures the script only runs if the mouse is present.
        if (Mouse.current != null)
        {
            //Gets the mouse position in screen space.
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            //Converts the screen position to viewport space.
            Vector2 viewportPosition = Camera.main.ScreenToViewportPoint(screenPosition);
            //Calculates the offset from the center of the screen.
            Vector2 offset = viewportPosition - new Vector2(0.5f, 0.5f);
            //Calculates the new position of the background based on the offset and the offset multiplier.
            transform.position = Vector3.SmoothDamp(transform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);
        }
    }
}
