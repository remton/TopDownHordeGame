using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActivate : MonoBehaviour
{
    public delegate void Activate();
    public event Activate EventPlayerActivate;

    public void OnActivateButton(InputAction.CallbackContext context) {
        if (context.action.triggered && EventPlayerActivate != null)
            EventPlayerActivate.Invoke();
    }

}
