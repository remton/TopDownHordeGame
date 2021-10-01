using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    // Remi here, I set up some placeholder functions and hooked them up to the input system.
    // If you want something to be done when input is entered You need to add the function to the PlayerInput coponent under the event tied to the input
    // Tutorial video on the input system https://www.youtube.com/watch?v=g_s0y5yFxYg&t=135s

    private Vector2 movementDir = Vector2.zero;
    private Vector2 lookDir = Vector2.up;
    private Vector2 mousePos = Vector2.zero;
    private bool isShooting = false;

    public void OnMove(InputAction.CallbackContext context) {
        movementDir = context.ReadValue<Vector2>();
        Debug.Log("Moveing: " + movementDir.ToString());
    }

    public void OnMousePos(InputAction.CallbackContext context) {
        mousePos = context.ReadValue<Vector2>();
        Debug.Log("Mouse Pos: " + mousePos.ToString());
    }

    public void OnLook(InputAction.CallbackContext context) {
        lookDir = context.ReadValue<Vector2>();
        Debug.Log("Looking: " + lookDir.ToString());
    }

    public void OnShoot(InputAction.CallbackContext context) {
        isShooting = context.action.triggered;
        Debug.Log("Shootin: " + isShooting);
    }

}
