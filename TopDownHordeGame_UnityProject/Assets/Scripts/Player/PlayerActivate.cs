using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActivate : MonoBehaviour
{
    public bool isPaused = false;

    public delegate void Activate(GameObject player);
    public event Activate EventPlayerActivate; // button pressed down
    public event Activate EventPlayerActivateRelease; // button is released

    public void OnActivateButton(InputAction.CallbackContext context) {
        if (!doInput)
            return;
        if (isPaused)
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

    public void OnOpenStatScreenButton(InputAction.CallbackContext context) {
        if (context.action.triggered) {
            StatScreen.OpenMenu();
        }
        else {
            StatScreen.CloseMenu();
        }
    }

    private void Awake() {
        GetComponent<Player>().EventDoInputChange += DoInputChange;
    }

    private bool doInput = true;
    private void DoInputChange(bool b) {
        doInput = b;
    }
}
