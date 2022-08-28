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
    private float iFrameTime;
    [SerializeField] [Tooltip("Amount healed each regen heal")]
    private float regenAmount;
    [SerializeField] [Tooltip("Time after damage before regen starts")]
    private float regenHitDelay; 
    [SerializeField] [Tooltip("Time between regen heals")]
    private float regenInterval;

    //components
    [SerializeField] private PlayerRevive playerRevive;
    private Timer timer;

    //prefabs
    [SerializeField] private AudioClip[] hurtsounds;
    [SerializeField] private GameObject hitEffectPrefab;

    //Other global vars
    [SyncVar] private float timeUntilDeath;
    private float timeSinceHit;
    private float timeSinceRegen;
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
        if (inIFrames || playerRevive.isBleedingOut || isDead)
            return;
        StartIFrames();
        health -= damageAmount;
        if (health <= 0)
            playerRevive.StartBleedingOut();
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
    /// <summary> [ClientRPC] Revives the player for all clients </summary>
    [ClientRpc]
    public void ReviveRPC() {

    }

    /// <summary> Respawns the player on this client </summary>
    [Client]
    public void Respawn() {
        if (playerRevive.isBleedingOut)
            playerRevive.Revive();
        health = maxHealth;
        isDead = false;
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
    public bool GetIsDead() { return isDead; }
    public bool GetIsBleedingOut() { return playerRevive.isBleedingOut; }
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

    //--- Dying ---
    #region Dying
    /// <summary> [Server] kills the client </summary>
    [Server]
    public void Kill() {
        Die();
    }
    [Server]
    private void Die() {
        DieRPC();
        //The player who owns this needs to call this method since they are the authority for this object
        PlayerManager.instance.OnPlayerDieRPC(gameObject);
    }
    /// <summary> [ClientRPC] Kills the player on all clients </summary>
    [ClientRpc]
    private void DieRPC() {
        isDead = true;
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
        RegenUpdate();
    }
    /// <summary> Updates regeneration by one frame </summary>
    private void RegenUpdate() 
    {
        if (playerRevive.isBleedingOut || isDead)
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
