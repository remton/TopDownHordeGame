using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour {

    //--- Private vars ---

    //values
    [SyncVar(hook = nameof(SyncHookHealth))] [SerializeField]
    private float health;
    [SyncVar(hook = nameof(SyncHookMaxHealth))] [SerializeField]
    private float maxHealth;
    [SerializeField] 
    private float reviveTime;
    [SerializeField] 
    private float iFrameTime;
    [SerializeField]
    private float bleedOutTime;
    [SerializeField] [Tooltip("Amount healed each regen heal")]
    private float regenAmount;
    [SerializeField] [Tooltip("Time after damage before regen starts")]
    private float regenHitDelay; 
    [SerializeField] [Tooltip("Time between regen heals")]
    private float regenInterval;

    //components
    [SerializeField] private BleedOutMeter bleedOutMeter;
    [SerializeField] private HitBoxController reviveTrigger;
    private Timer timer;

    //prefabs
    [SerializeField] private AudioClip reviveSound;
    [SerializeField] private AudioClip[] hurtsounds;
    [SerializeField] private GameObject hitEffectPrefab;

    //Other global vars
    private GameObject reviver;
    [SyncVar] private float timeUntilDeath;
    private float timeSinceHit;
    private float timeSinceRegen;
    private Guid reviveTimerID = Guid.Empty;
    private bool isBeingRevived;
    private bool isBleedingOut;
    private bool hasRevivePrompt;
    private bool inIFrames = false;
    private bool isDead = false;


    // --- Public Events ---
    public delegate void HealthChanged(float health, float max);
    public event HealthChanged EventHealthChanged;
    //if(EventHealthChanged != null){EventHealthChanged.Invoke(health, maxHealth); }


    // --- Public Methods ---

    /// <summary> [Server] Heals by healAmount up to maxHealth </summary>
    [Server]
    public void Heal(float healAmount) {
        float newHealth = health + healAmount;
        if (newHealth > maxHealth)
            newHealth = maxHealth;
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    /// <summary> [Command] Damages health by given amount </summary>
    [Command(requiresAuthority = false)]
    public void DamageCMD(float damageAmount) {
        if (inIFrames || isBleedingOut || isDead)
            return;
        StartIFrames();
        health -= damageAmount;
        if (health <= 0)
            GoDownRPC();
        timeSinceHit = 0;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
        GameObject obj = Instantiate(hitEffectPrefab);
        obj.transform.position = transform.position;
        NetworkServer.Spawn(obj);
        AudioManager.instance.PlaySound(hurtsounds[UnityEngine.Random.Range(0, hurtsounds.Length)]);
    }

    /// <summary> [Server] Revives the player </summary>
    [Server]
    public void Revive() {
        ReviveRPC();
    }
    /// <summary> [Command] Revives the player </summary>
    [Command(requiresAuthority = false)]
    public void ReviveCMD() {
        ReviveRPC();
    }
    /// <summary> [ClientRPC] Revives the player for all clients </summary>
    [ClientRpc]
    public void ReviveRPC() {
        if (reviver == null || !reviver.GetComponent<PlayerHealth>().GetIsBleedingOut()) {
            AudioManager.instance.PlaySound(reviveSound);
            reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
            reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
            isBleedingOut = false;
            isBeingRevived = false;
            health = maxHealth;
            bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
            GetComponent<PlayerMovement>().EnableMovement();
            //Debug.Log("Revived!");
            if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
        }
        StopRevive(reviver);
        reviver = null;
    }

    /// <summary> Respawns the player on this client </summary>
    [Client]
    public void Respawn() {
        health = maxHealth;
        isDead = false;
        isBeingRevived = false;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        reviveTimerID = Guid.Empty;
        GetComponent<PlayerMovement>().EnableMovement();
        //GetComponent<PlayerWeaponControl>().ResetWeapons();
        //Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    } 

    /// <summary> Changes maxHealth by a given multiplier </summary>
    public void ChangeMaxHealth(float multiplier) {
        maxHealth = Mathf.RoundToInt(multiplier * maxHealth);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    /// <summary> Changes regeneration speed by a given multiplier </summary>
    public void ChangeRegenValues(float multiplier) {
        regenHitDelay *= multiplier;
        regenInterval *= multiplier;
    }

    public float GetHealthRatio() {
        return (float)health / maxHealth;
    }
    public float GetBleedOutTimeRatio() {
        return timeUntilDeath / bleedOutTime;
    }
    public bool GetIsDead() { return isDead; }
    public bool GetIsBleedingOut() { return isBleedingOut; }
    public float GetMaxHealth() { return maxHealth; }

    //------ Private methods ------

    //--- Syncvar Hooks ---
    #region Sync Hooks
    /// <summary> Hook for Syncvar health. Called on clients when changed </summary>
    [Client]
    private void SyncHookHealth(float oldHealth, float newHealth) {
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    /// <summary> Hook for Syncvar maxHealth. Called on clients when changed </summary>
    [Client]
    private void SyncHookMaxHealth(float oldMax, float newMax) {
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    #endregion

    //--- Revive Trigger ---
    #region Revive trigger
    /// <summary> [Client] called on client when player enters the revive trigger</summary>
    [Client]
    private void OnPlayerEnterReviveTrigger(GameObject otherPlayer) {
        OnPlayerEnterReviveTriggerCMD(otherPlayer);
    }
    [Command(requiresAuthority = false)]
    private void OnPlayerEnterReviveTriggerCMD(GameObject otherPlayer) {
        OnPlayerEnterReviveTriggerRPC(otherPlayer);
    }
    /// <summary> [ClientRpc] called on clients when a player enters the revive trigger</summary>
    [ClientRpc]
    private void OnPlayerEnterReviveTriggerRPC(GameObject otherPlayer) {
        hasRevivePrompt = true;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate += StartRevive;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease += StopRevive;
    }
    /// <summary> [Client] called on client when player exits the revive trigger</summary>
    [Client]
    private void OnPlayerExitReviveTrigger(GameObject otherPlayer) {
        OnPlayerExitReviveTriggerCMD(otherPlayer);
    }
    /// <summary> [Command] called on server when a player leaves the revive trigger </summary>
    [Command(requiresAuthority = false)]
    private void OnPlayerExitReviveTriggerCMD(GameObject otherPlayer) {
        if (isBeingRevived && otherPlayer == reviver) {
            StopReviveTRPC(otherPlayer.GetComponent<NetworkIdentity>().connectionToClient, otherPlayer);
        }
        OnPlayerExitReviveTriggerRPC(otherPlayer);
    }
    /// <summary> [ClientRpc] called on clients when a player exits the revive trigger</summary>
    [ClientRpc]
    private void OnPlayerExitReviveTriggerRPC(GameObject otherPlayer) {
        hasRevivePrompt = false;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate -= StartRevive;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease -= StopRevive;
    }
    #endregion

    //--- Reviving ---
    #region reviving
    /// <summary> [Client] Called on revivers client client when starting to revive this player </summary>
    [Client]
    private void StartRevive(GameObject reviver) {
        if (isBleedingOut && !reviver.GetComponent<PlayerHealth>().isBleedingOut) {
            isBeingRevived = true;
            timer.KillTimer(reviveTimerID);
            reviveTimerID = timer.CreateTimer(reviveTime, ReviveCMD);
            StartReviveCMD(reviver);
        }
    }
    /// <summary> [Command] called when reviver starts to revive this player </summary>
    [Command(requiresAuthority = false)]
    private void StartReviveCMD(GameObject reviver) {
        this.reviver = reviver;
        isBeingRevived = true;
        StartReviveRPC(reviver);
    }
    /// <summary> [ClientRPC] called on all clients when reviver starts to revive this player </summary>
    [ClientRpc]
    private void StartReviveRPC(GameObject reviver) {
        this.reviver = reviver;
        isBeingRevived = true;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
    }

    /// <summary> [Client] Called on this client when stopping revive of this player </summary>
    [Client]
    private void StopRevive(GameObject reviver) {
        if(isBleedingOut && isBeingRevived) {
            isBeingRevived = false;
            timer.KillTimer(reviveTimerID);
            reviveTimerID = Guid.Empty;
            StopReviveCMD(reviver);
        }
    }
    /// <summary> [Command] called on server when reviver stops revive of this player </summary>
    [Command(requiresAuthority = false)]
    private void StopReviveCMD(GameObject reviver) {
        isBeingRevived = false;
        this.reviver = null;
        StopReviveRPC(reviver);
    }
    /// <summary> [ClientRPC] called on all clients when reviver stops revive of this player </summary>
    [ClientRpc]
    private void StopReviveRPC(GameObject reviver) {
        isBeingRevived = false;
        this.reviver = null;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
    }
    /// <summary> [TargetRpc] Tells the client with connection to stop reviving this player </summary>
    [TargetRpc] void StopReviveTRPC(NetworkConnection network, GameObject reviver) {
        StopRevive(reviver);
    }
    #endregion

    //--- Going Down ---
    #region Going Down
    /// <summary> [Server] server starts bleeding out state </summary>
    [Server]
    private void GoDownCMD() {
        GoDownRPC();
    }
    /// <summary> [ClientRpc] Starts bleeding out state for all clients </summary>
    [ClientRpc]
    private void GoDownRPC() {
        //Debug.Log("Bleeding out!");
        reviveTrigger.EventObjEnter += OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit += OnPlayerExitReviveTrigger;
        reviveTrigger.ForceEntry();
        timeUntilDeath = bleedOutTime;
        isBleedingOut = true;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        GetComponent<PlayerMovement>().DisableMovement();
        foreach (GameObject revivee in reviveTrigger.Hits()) {
            revivee.GetComponent<PlayerHealth>().StopRevive(gameObject);
            StopRevive(revivee);
        }
    }
    #endregion

    //--- Dying ---
    #region Dying
    /// <summary> [Server] kills the client </summary>
    [Server]
    private void Die() {
        DieRPC();
        //The player who owns this needs to call this method since they are the authority for this object
        PlayerManager.instance.OnPlayerDieRPC(gameObject);
    }
    /// <summary> [ClientRPC] Kills the player on all clients </summary>
    [ClientRpc]
    private void DieRPC() {
        reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
        isBleedingOut = false;
        isDead = true;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
    }
    #endregion

    //--- IFrames ---
    #region IFrames
    private void StartIFrames() {
        timer.CreateTimer(iFrameTime, EndIFrames);
        inIFrames = true;
    }
    private void EndIFrames() {
        inIFrames = false;
    }
    #endregion

    //Called immediatly when this script object is created
    private void Awake() {
        health = maxHealth;
        timer = GetComponent<Timer>();
    }
    //Called on first frame in scene
    private void Start() {
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    //Called on first frame in scene for clients
    public override void OnStartClient() {
        base.OnStartClient();
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    //Called every frame
    private void Update() {
        //Is bleeding out timer
        BleedOutUpdate();
        RegenUpdate();
    }
    /// <summary> Updates Bleeding out by one frame </summary>
    private void BleedOutUpdate() {
        if (isBleedingOut) {
            if (!isBeingRevived)
                timeUntilDeath -= Time.deltaTime;
            if (timeUntilDeath <= 0) {
                if(isServer)
                    Die();
            }
        }
    }
    /// <summary> Updates regeneration by one frame </summary>
    private void RegenUpdate() 
    {
        if (isBleedingOut || isDead)
            return;
        if (timeSinceHit < regenHitDelay)
            timeSinceHit += Time.deltaTime;
        else if (timeSinceRegen < regenInterval)
            timeSinceRegen += Time.deltaTime;
        else{
            if(isServer)
                Heal(regenAmount);
            timeSinceRegen = 0;
        }
    }
}
