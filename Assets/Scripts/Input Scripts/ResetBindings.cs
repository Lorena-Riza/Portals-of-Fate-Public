using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAction;

    //when the reset button is pressed the bindings are reset to default
    public void ResetAll()
    {
        foreach (InputActionMap map in inputAction.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }
}
