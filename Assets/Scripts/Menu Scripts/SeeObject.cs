using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//It hides the object at the start and shows it when the ShowObject method is called.
public class SeeObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; //Reference to the sprite renderer component.

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Hides the object at the start.
        spriteRenderer.enabled = false;
    }

    //This method is called to show the object.
    public void ShowObject()
    {
        spriteRenderer.enabled = true;
    }
}
