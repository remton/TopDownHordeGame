using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script handles player movement and direction 


// Remi here, I set up some placeholder functions and hooked them up to the input system.
// If you want something to be done when input is entered You need to add the function to the PlayerInput component under the event tied to the input
// Tutorial video on the input system https://www.youtube.com/watch?v=g_s0y5yFxYg&t=135s


public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;

    [SerializeField] private Camera camera;
    [SerializeField] private Rigidbody2D rb;

    private bool doMovement = true;

    private bool isRunning = false;
    private Vector2 moveDir;
    private Vector2 lookDir;
    private Vector2 mousePos;

    // If mouse input was detected this is true if gamepad this is false
    private bool useMouseToLook;

    // THis is used by other scripts to access what direction the player is looking
    private Vector2 currentLookDir;
    public Vector2 GetCurrentLookDir() { return currentLookDir; }

    private void Update() {
        if (useMouseToLook)
            LookAtMouse();
    }

    //called after every frame
    private void FixedUpdate() {
        if(doMovement)
            Move(moveDir);
    }

    public void DisableMovement() {
        doMovement = false;
    }

    public void EnableMovement() {
        doMovement = true;
    }

    // Called whenever a change in movement input. Moves the player based in walk and run speed
    public void OnMove(InputAction.CallbackContext context) {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveDir = moveInput.normalized;
    }

    // called whenever mouse position input event is called (Keyboard inputs only)
    public void OnMousePos(InputAction.CallbackContext context) {
        useMouseToLook = true;
        Vector2 mouseScreenPos = context.ReadValue<Vector2>();
        mousePos = camera.ScreenToWorldPoint(mouseScreenPos);
        LookAtMouse();
    }

    // called whenever a change in look direction input event is called (gamepad inputs only)
    public void OnLook(InputAction.CallbackContext context) {

        useMouseToLook = false;
        if (context.ReadValue<Vector2>() == Vector2.zero)
            return;
        LookInDir(context.ReadValue<Vector2>());
    }

    // called whenever run input event is called
    public void OnRun(InputAction.CallbackContext context) {
        isRunning = context.action.triggered;
    }

    // Faces the player towards the given direction vector
    private void LookInDir(Vector2 lookDir2D) {
        Vector3 lookDir3D = new Vector3(lookDir2D.x, lookDir2D.y, transform.position.z);
        transform.right = lookDir3D;
        currentLookDir = lookDir2D;
    }

    // Faces player towards the mousePos vector
    private void LookAtMouse() {
        Vector2 myPos = transform.position;
        Vector2 dir = mousePos - myPos;
        LookInDir(dir);
    }

    // Moves the player based on the given direction
    // Should be called in fixed update
    private void Move(Vector2 movementDir) {
        Vector2 newPos = transform.position;

        if (isRunning)
            newPos += runSpeed * movementDir * Time.fixedDeltaTime;
        else
            newPos += walkSpeed * movementDir * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

}
