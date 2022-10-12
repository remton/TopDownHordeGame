using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

// This script handles player movement and direction 


// Remi here, I set up some placeholder functions and hooked them up to the input system.
// If you want something to be done when input is entered You need to add the function to the PlayerInput component under the event tied to the input
// Tutorial video on the input system https://www.youtube.com/watch?v=g_s0y5yFxYg&t=135s


public class PlayerMovement : NetworkBehaviour {
    private Animator animator;

    public bool isPaused = false;
    private bool isMoving = false;
    private bool isRunning = false;
    //How long since the player last moved
    public float lastMoved { get; internal set; }

    //Called when stamina value changes
    public delegate void StaminaChange(float newStamina, float newMax);
    public event StaminaChange EventStaminaChange;

    private bool knockBackActive;
    private bool forceknockbackActive;
    [SerializeField] private float minSpeedForEndKnockback;
    private Timer timer;

    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float runSpeed = 4;
    [SerializeField] private float staminaRegenRateWalking = .01375F;
    [SerializeField] private float staminaRegenRateStanding = .0275F;
    [SerializeField] private float staminaDrainRate = .05F;
    [SerializeField] private float staminaMaximum = 10F;
    private float staminaRemaining = 10F;

    [Command(requiresAuthority = false)]
    private void CallStaminaChangeEvent(float stamina, float max) {
        OnStaminaRatioChange(stamina, max);
    }
    [ClientRpc]
    private void OnStaminaRatioChange(float stamina, float max) {
        if (EventStaminaChange != null) { EventStaminaChange.Invoke(stamina, max); }
    }


    private bool wentBelowThreshold = false;
    private float staminaThreshold = 0F;
    private bool stillRunning = true;

    public List<float> walkSpeedMultipliers = new List<float>();
    public List<float> runSpeedMultipliers = new List<float>();

    [SerializeField] private Rigidbody2D rb;

    private bool doMovement = true;
    private Vector2 moveDir;
    public Vector2 mouseScreenPos;
    
    public float movementMult = 1;

    // If mouse input was detected this is true if gamepad this is false
    private bool useMouseToLook;

    /// <summary> returns the ratio of current stamina to max stamina  </summary>
    public float GetStaminaRatio() { return staminaRemaining / staminaMaximum; }


    private Vector2 currentLookDir;
    public Vector2 GetCurrentLookDir() { return currentLookDir; }
    private void SetCurrentLookDir(Vector2 newDir) { 
        currentLookDir = newDir;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        timer = GetComponent<Timer>();
        staminaThreshold = staminaMaximum * 0.2F;
        GetComponent<Player>().EventDoInputChange += DoInputChange;
    }

    private void Start() {
        CallStaminaChangeEvent(staminaRemaining, staminaMaximum);
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
        if (isPaused)
            return;

        if (useMouseToLook) {
            LookAtMouse();
        }
        UpdateAnimation();
        //update last moved time
        if (isMoving)
            lastMoved = 0;
        else
            lastMoved += Time.deltaTime;
    }

    //called after every frame
    private void FixedUpdate() {
        if (isPaused)
            return;

        if (doMovement && !knockBackActive && !forceknockbackActive)
            Move(moveDir);

        if (knockBackActive && !forceknockbackActive && rb.velocity.magnitude <= minSpeedForEndKnockback) {
            knockBackActive = false;
        }
        // Debug.Log("isMoving: " + isMoving);
    }


    [Command(requiresAuthority = false)]
    public void KnockbackCMD(float strength, Vector2 dir) {
        KnockbackTRPC(connectionToClient, strength, dir);
    }
    [TargetRpc]
    private void KnockbackTRPC(NetworkConnection conn, float strength, Vector2 dir) {
        KnockBack(strength, dir);
    }
    [Client]
    public void KnockBack(float strength, Vector2 dir) {
        knockBackActive = true;
        forceknockbackActive = true;
        rb.AddForce(dir.normalized * strength);
        timer.CreateTimer(Time.fixedDeltaTime * 2, EndKnockbackTimer);
    }
    private void EndKnockbackTimer() {
        forceknockbackActive = false;
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
        if (!doInput)
            return;

        //Debug.Log("PlayerCon [" + GetComponent<Player>().GetConnection().netId + "] moving . . .");
        //Debug.Log("Con [" + netId + "] moving . . .");
        Vector2 moveInput = context.ReadValue<Vector2>();
        moveDir = moveInput.normalized;
    }

    [Client]
    private void SetLookDir(Vector2 dir) {
        SetCurrentLookDir(dir);
        float xScale = transform.localScale.x;
        //if the sign of the direction isn't the same as the sign of x scale 
        if ((dir.x < 0) != (xScale < 0)) {
            transform.localScale = new Vector3(-1* transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
    private void LookAtMouse() {
        if (isPaused)
            return;

        Vector2 myPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 dir = mousePos - myPos;
        SetLookDir(dir);
    }
    private void LookTowards(Vector2 lookDir2D) {
        if (isPaused)
            return;
        SetLookDir(lookDir2D);
    }
    private void UpdateAnimation() {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
    }

    // called whenever mouse position input event is called (Keyboard inputs only)
    public void OnMousePos(InputAction.CallbackContext context)
    {
        if (!doInput)
            return;

        if (Camera.main == null)
        {
            //Debug.Log("Camera gone?! \\o_0/");
            return;
        }

        useMouseToLook = true;
        mouseScreenPos = context.ReadValue<Vector2>();
        LookAtMouse();
    }

    // called whenever a change in look direction input event is called (gamepad inputs only)
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!doInput)
            return;

        useMouseToLook = false;
        if (context.ReadValue<Vector2>() == Vector2.zero)
            return;
        SetLookDir(context.ReadValue<Vector2>());
    }

    // called whenever run input event is called
    public void OnRun(InputAction.CallbackContext context)
    {
        if (!doInput)
            return;

        isRunning = context.action.triggered;
    }


    private void RegenStamina(Vector2 movementDir)
    {
        if ((!isRunning || !stillRunning) && staminaRemaining < staminaMaximum)
        {
            if (movementDir.x != 0 || movementDir.y != 0)
            {
                staminaRemaining += staminaRegenRateWalking;
            }
            else
                staminaRemaining += staminaRegenRateStanding;
            if (staminaRemaining > staminaThreshold) {
                wentBelowThreshold = false;
                stillRunning = true;
            }
            CallStaminaChangeEvent(staminaRemaining, staminaMaximum);
        }
    }

    // Moves the player based on the given direction
    // Should be called in fixed update
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;

        if (movementDir.x != 0 || movementDir.y != 0)
        {
            isMoving = true;
            if (isRunning && staminaRemaining > 0 && stillRunning)
            {
                newPos += runSpeedMult() * runSpeed * movementDir * Time.fixedDeltaTime;
                staminaRemaining -= staminaDrainRate;
                if (staminaRemaining < staminaThreshold)
                {
                    wentBelowThreshold = true;
                }
                CallStaminaChangeEvent(staminaRemaining, staminaMaximum);
            }
            else
            {
                newPos += walkSpeedMult() * walkSpeed * movementDir * Time.fixedDeltaTime;
                if (wentBelowThreshold) {
                    stillRunning = false; 
                }
            }
            rb.MovePosition(newPos);
        }
        else {
            isMoving = false;
        }
        RegenStamina(movementDir);
    }
    private float walkSpeedMult() { // Calculate the walk speed multiplier of the player. 
        float multSumFast = 0;
        float multSumSlow = 0;
        foreach (float num in walkSpeedMultipliers) {
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
    private float runSpeedMult() { // Fixed: Bolt perk combined with M1911 weapon speed no longer allows the player to sprint through zombies and take 0 damage. Changed calculation for the multiplier to fix this.  
        float multSumFast = 0;
        float multSumSlow = 0;
        foreach (float num in runSpeedMultipliers) {
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
    public void ChangeRegenValues(float balanceMult) { // This changes the stamina regeneration rates for standing and walking
        staminaRegenRateStanding = staminaRegenRateStanding * balanceMult;
        staminaRegenRateWalking = staminaRegenRateWalking * balanceMult;
    }
    public void ChangeMaximumStamina(float maximumStaminaMultiplier) // This changes the maximum stamina and the stamina sprint threshold
    {
        staminaMaximum = staminaMaximum * maximumStaminaMultiplier;
        staminaThreshold = staminaThreshold * maximumStaminaMultiplier;
        CallStaminaChangeEvent(staminaRemaining, staminaMaximum);
    }

    private bool doInput = true;
    private void DoInputChange(bool b) {
        doInput = b;
    }
}

