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
    private Timer timer;
    private PlayerBleedout bleedout;
    private PlayerRevive revive;

    //prefabs
    [SerializeField] private AudioClip reviveSound;
    [SerializeField] private AudioClip[] hurtsounds;
    [SerializeField] private GameObject hitEffectPrefab;

    //Other global vars
    private GameObject reviver;
    [SyncVar] private float timeUntilDeath;
    private float timeSinceHit;
    private float timeSinceRegen;
    private bool inIFrames = false;
    private bool isDead = false;
    private bool doFriendlyFire = false;


    // --- Public Events ---
    public delegate void HealthChanged(float health, float max);
    public event HealthChanged EventHealthChanged;
    //if(EventHealthChanged != null){EventHealthChanged.Invoke(health, maxHealth); }

    #region Unity Control
    //Called immediatly when this script object is created
    private void Awake() {
        health = maxHealth;
        timer = GetComponent<Timer>();
        revive = GetComponent<PlayerRevive>();
        bleedout = GetComponent<PlayerBleedout>();
        bleedout.EventOnBleedoutEnd += OnBleedout;
        revive.EventOnRevive += OnRevive;
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
    #endregion

    #region Incoming Events
    [Server]
    private void OnRevive() {
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    [Server]
    private void OnBleedout() {
        Die();
    }
    #endregion

    #region Syncvar Hooks
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

    #region Friendly Fire
    [Server]
    public void SetFriendlyFire(bool b) {
        doFriendlyFire = b;
        SetFriendlyFireRPC(b);
    }
    [ClientRpc]
    public void SetFriendlyFireRPC(bool b) {
        doFriendlyFire = b;
    }
    public bool HasFriendlyFire() {
        return doFriendlyFire;
    }
    #endregion

    #region Healing
    /// <summary> [Server] Heals by healAmount up to maxHealth </summary>
    [Server]
    public void Heal(float healAmount) {
        float newHealth = health + healAmount;
        if (newHealth > maxHealth)
            newHealth = maxHealth;
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    /// <summary> Updates regeneration by one frame </summary>
    private void RegenUpdate() {
        if (bleedout.isBleedingOut || isDead)
            return;
        if (timeSinceHit < regenHitDelay)
            timeSinceHit += Time.deltaTime;
        else if (timeSinceRegen < regenInterval)
            timeSinceRegen += Time.deltaTime;
        else {
            if (isServer)
                Heal(regenAmount);
            timeSinceRegen = 0;
        }
    }    
    /// <summary> Changes regeneration speed by a given multiplier </summary>
    public void ChangeRegenValues(float multiplier) {
        regenHitDelay *= multiplier;
        regenInterval *= multiplier;
    }
    /// <summary> Changes maxHealth by a given multiplier </summary>
    public void ChangeMaxHealth(float multiplier) {
        maxHealth = Mathf.RoundToInt(multiplier * maxHealth);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    public float GetMaxHealth() { 
        return maxHealth; 
    }
    #endregion

    #region Damage
    /// <summary> [Command] Damages health by given amount </summary>
    [Command(requiresAuthority = false)]
    public void DamageCMD(float damageAmount) {
        Damage(damageAmount, true, true, true);
    }
    [Command(requiresAuthority = false)]
    public void DamageCMD(float damageAmount, bool doIFrames, bool doEffect, bool effectRegen) {
        Damage(damageAmount, doIFrames, doEffect, effectRegen);
    }
    [Server]
    private void Damage(float damageAmount, bool doIFrames, bool doEffect, bool effectRegen) {
        if (doIFrames && inIFrames)
            return;
        if (bleedout.isBleedingOut || isDead)
            return;
        if(doIFrames)
            StartIFrames();
        health -= damageAmount;
        //Health reach zero we start bleeding out
        if (health <= 0)
            bleedout.StartBleedout();
        if(effectRegen)
            timeSinceHit = 0;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
        if (doEffect) {
            GameObject obj = Instantiate(hitEffectPrefab);
            obj.transform.position = transform.position;
            NetworkServer.Spawn(obj);
            AudioManager.instance.PlaySound(hurtsounds[UnityEngine.Random.Range(0, hurtsounds.Length)]);
        }
    }
    #endregion

    #region IFrames
    private void StartIFrames() {
        timer.CreateTimer(iFrameTime, EndIFrames);
        inIFrames = true;
    }
    private void EndIFrames() {
        inIFrames = false;
    }
    #endregion

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
        isDead = true;
    }
    public bool GetIsDead() { 
        return isDead; 
    }
    #endregion

    #region Respawn
    /// <summary> Respawns the player on this client </summary>
    [Client]
    public void Respawn() {
        health = maxHealth;
        isDead = false;
        GetComponent<PlayerMovement>().EnableMovement();
        //GetComponent<PlayerWeaponControl>().ResetWeapons();
        //Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    #endregion
   
}
