using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the cursor in the game, changing its appearance based on user interaction.
public class CursorController : MonoBehaviour
{

    public Texture2D cursor; //Texture for the normal cursor.
    public Texture2D cursorClick; //Texture for the cursor when clicked.

    private CursorControlls controls; //Input system for handling mouse events.


    private void Awake()
    {
        // Initialize the cursor textures.
        controls = new CursorControlls();
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;

    }

    //Enable input detection.
    private void OnEnable()
    {
        controls.Enable();
    }

    //Disable input detection.
    private void OnDisable()
    {
        controls.Disable();
    }

    //Change the cursor texture based on the provided Texture2D with a centering hotspot.
    private void ChangeCursor(Texture2D cursorType)
    {
        Vector2 hotspot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Subscribe to mouse click start/cancel events.
        controls.Mouse.Click.started += _ => StartedClick();
        controls.Mouse.Click.canceled += _ => CanceledClick();
    }

    //When click starts, use the click cursor.
    private void StartedClick()
    {
        ChangeCursor(cursorClick);
    }

    //When click ends, revert to the normal cursor.
    private void CanceledClick()
    {
        ChangeCursor(cursor);
    }
}
