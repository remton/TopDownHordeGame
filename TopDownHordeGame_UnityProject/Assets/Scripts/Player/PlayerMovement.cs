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
    private float walkSpeed = 2;
    private float runSpeed = 4;

    private float staminaRemaining = 10F;
    private float staminaMaximum = 10F;
    private float staminaRegenRateWalking = .01375F;
    private float staminaRegenRateStanding = .0275F;
    private float staminaDrainRate = .05F;

    private bool wentBelowThreshold = false;
    private float staminaThreshold = 2;
    private bool stillRunning = true;

    /// <summary> returns the ratio of current stamina to max stamina  </summary>
    public float GetStaminaRatio()
    {
        return staminaRemaining / staminaMaximum;
    }

    public List<float> walkSpeedMultipliers = new List<float>();
    public List<float> runSpeedMultipliers = new List<float>();
    private float walkSpeedMult()
    {
        float multSumFast = 0;
        float multSumSlow = 0;
        foreach (float num in walkSpeedMultipliers)
        {
            if (num < 1)
                multSumSlow += 1 / num;
            else
                multSumFast += num;
        }
        if (multSumFast == 0)
            multSumFast = 1;
        if (multSumSlow == 0)
            multSumSlow = 1;
        return multSumFast * (1 / (multSumSlow));
    }
    private float runSpeedMult()
    { // Fixed: Bolt perk combined with M1911 weapon speed no longer allows the player to sprint through zombies and take 0 damage. Changed calculation for the multiplier to fix this.  
        float multSumFast = 0;
        float multSumSlow = 0;
        foreach (float num in runSpeedMultipliers)
        {
            if (num < 1)
                multSumSlow += 1 / num;
            else
                multSumFast += num - 1; // If the -1 is not here, the player speed quickly spirals out of control when stacking positive buffs. 
        }
        multSumFast++; // This makes the player fast multiplier be at least one 
                       //        if (multSumFast == 0)
                       //            multSumFast = 1;
        if (multSumSlow == 0)
            multSumSlow = 1;
        return multSumFast * (1 / (multSumSlow));
    }


    [SerializeField] private Rigidbody2D rb;
    private Camera mainCamera;

    private bool doMovement = true;

    private bool isRunning = false;
    private Vector2 moveDir;
    private Vector2 lookDir; // This is not being used 
    public Vector2 mouseScreenPos;

    // If mouse input was detected this is true if gamepad this is false
    private bool useMouseToLook;

    // THis is used by other scripts to access what direction the player is looking
    private Vector2 currentLookDir;
    public Vector2 GetCurrentLookDir() { return currentLookDir; }

    public float movementMult = 1;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnDeviceChange(InputDevice device, InputDeviceChange deviceChange)
    {
        string deviceClass = device.description.deviceClass;
        if (deviceClass == "keyboard")
        {
            Debug.Log("Keyboard configured");
            useMouseToLook = true;
        }
        else if (deviceClass == "gamepad")
        {
            Debug.Log("Gamepad configured");
            useMouseToLook = false;
        }
        else
        {
            Debug.Log("input calss " + deviceClass + "not recognized");
        }
    }

    private void Update()
    {
        if (useMouseToLook)
            LookAtMouse();
    }

    //called after every frame
    private void FixedUpdate()
    {
        if (doMovement)
            Move(moveDir);
    }

    public void DisableMovement()
    {
        doMovement = false;
    }

    public void EnableMovement()
    {
        doMovement = true;
    }

    // Called whenever a change in movement input. Moves the player based in walk and run speed
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveDir = moveInput.normalized;
    }

    // called whenever mouse position input event is called (Keyboard inputs only)
    public void OnMousePos(InputAction.CallbackContext context)
    {
        if (Camera.main == null)
        {
            Debug.Log("Camera gone?! \\o_0/");
            return;
        }

        useMouseToLook = true;
        mouseScreenPos = context.ReadValue<Vector2>();
        LookAtMouse();
    }

    // called whenever a change in look direction input event is called (gamepad inputs only)
    public void OnLook(InputAction.CallbackContext context)
    {
        useMouseToLook = false;
        if (context.ReadValue<Vector2>() == Vector2.zero)
            return;
        LookInDir(context.ReadValue<Vector2>());
    }

    // called whenever run input event is called
    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.action.triggered;
    }

    // Faces the player towards the given direction vector
    private void LookInDir(Vector2 lookDir2D)
    {
        Vector3 lookDir3D = new Vector3(lookDir2D.x, lookDir2D.y, transform.position.z);
        transform.right = lookDir3D;
        currentLookDir = lookDir2D;
    }

    // Faces player towards the mousePos vector
    private void LookAtMouse()
    {
        Vector2 myPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 dir = mousePos - myPos;
        LookInDir(dir);
    }
    private void RegenStamina(Vector2 movementDir)
    {
        if (!isRunning && staminaRemaining < staminaMaximum)
        {
            if (movementDir.x != 0 || movementDir.y != 0)
            {
                staminaRemaining += staminaRegenRateWalking;
            }
            else
                staminaRemaining += staminaRegenRateStanding;
            if (staminaRemaining > staminaThreshold)
            {
                wentBelowThreshold = false;
                stillRunning = true;
            }
        }
    }

    // Moves the player based on the given direction
    // Should be called in fixed update
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;

        if (movementDir.x != 0 || movementDir.y != 0)
        {
            if (isRunning && staminaRemaining > 0 && stillRunning)
            {
                newPos += runSpeedMult() * runSpeed * movementDir * Time.fixedDeltaTime;
                staminaRemaining -= staminaDrainRate;
                if (staminaRemaining < staminaThreshold)
                {
                    wentBelowThreshold = true;
                }
            }
            else
            {
                newPos += walkSpeedMult() * walkSpeed * movementDir * Time.fixedDeltaTime;
                if (wentBelowThreshold)
                {
                    stillRunning = false;
                }
            }
            rb.MovePosition(newPos);
        }
        RegenStamina(movementDir);
    }
}

