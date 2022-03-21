using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActivate : MonoBehaviour
{
    public bool isDisabled = false;

    public delegate void Activate(GameObject player);
    public event Activate EventPlayerActivate; // button pressed down
    public event Activate EventPlayerActivateRelease; // button is released

    public void OnActivateButton(InputAction.CallbackContext context) {
        if (isDisabled)
            return;

        if (context.action.triggered) {
            if(EventPlayerActivate != null)
                EventPlayerActivate.Invoke(gameObject);
        }
        else {
            if(EventPlayerActivateRelease != null)
            EventPlayerActivateRelease.Invoke(gameObject);
        }
    }

}
