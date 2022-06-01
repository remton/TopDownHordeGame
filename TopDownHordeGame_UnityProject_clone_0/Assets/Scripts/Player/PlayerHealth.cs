using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour {
    private Timer timer;
    public bool isBeingRevived;
    private GameObject reviver;
    [SerializeField] private AudioClip reviveSound;
    [SerializeField] private AudioClip[] hurtsounds;

    [SerializeField] private float reviveTime;
    [SerializeField] private HitBoxController reviveTrigger;
    public RevivePrompt revivePrompt;
    private Guid reviveTimerID = Guid.Empty;

    public float bleedOutTime;

    private float timeUntilDeath;
    private bool isBleedingOut;
    private float regenAmount = 1;
    private float regenHitDelay = 10; 
    private float regenInterval = 4;
    private float timeSinceHit;
    private float timeSinceRegen;
    public GameObject hitEffectPrefab;
    private bool inIFrames = false;
    [SerializeField] private float iFrameTime;

    [SyncVar(hook = nameof(OnHealthChange))]
    private float health;
    [SyncVar(hook = nameof(OnMaxHealthChange))]
    [SerializeField] private float maxHealth;

    [Client]
    private void OnHealthChange(float oldHealth, float newHealth) {
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    [Client]
    private void OnMaxHealthChange(float oldMax, float newMax) {
        maxHealth = newMax;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    [Command]
    private void OnPlayerEnterReviveTrigger(GameObject otherPlayer) {
        OnPlayerEnterReviveTrigger_TargetRPC(otherPlayer.GetComponent<Player>().connectionToClient, otherPlayer);
    }
    [TargetRpc]
    private void OnPlayerEnterReviveTrigger_TargetRPC(NetworkConnection connection, GameObject otherPlayer) {
        revivePrompt.Activate();
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate += OnReviveActivateDown;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease += OnReviveActivateRelease;
    }

    [Command]
    private void OnPlayerExitReviveTrigger(GameObject otherPlayer) {
        OnPlayerExitReviveTrigger_TargetRPC(otherPlayer.GetComponent<Player>().connectionToClient, otherPlayer);
    }
    [TargetRpc]
    private void OnPlayerExitReviveTrigger_TargetRPC(NetworkConnection connection, GameObject otherPlayer) {
        revivePrompt.Deactivate();
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate -= OnReviveActivateDown;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease -= OnReviveActivateRelease;
        if (reviveTimerID != Guid.Empty) {
            timer.KillTimer(reviveTimerID);
            isBeingRevived = false;
        }
        
    }
    private void Awake() {
        timer = GetComponent<Timer>();
    }

    public float GetHealthRatio() {
        return (float)health / maxHealth;
    }

    private void OnReviveActivateDown(GameObject otherPlayer) {
        if (isBleedingOut && !otherPlayer.GetComponent<PlayerHealth>().isBleedingOut) {
            Debug.Log("Player start revive");
            if (reviveTimerID != Guid.Empty) {
                timer.KillTimer(reviveTimerID);
                reviveTimerID = Guid.Empty;
            }
            reviveTimerID = timer.CreateTimer(reviveTime, Revive);
            reviver = otherPlayer;
            isBeingRevived = true;
        }
    }
    private void OnReviveActivateRelease(GameObject otherPlayer) {
        if(isBleedingOut && reviveTimerID != Guid.Empty) {
            Debug.Log("Player end revive");
            timer.KillTimer(reviveTimerID);
            isBeingRevived = false;
            reviveTimerID = Guid.Empty;
        }
    }



    [SyncVar] private bool isDead = false;
    public bool GetIsDead() { return isDead; }

    public bool GetIsBleedingOut() { return isBleedingOut; }

    public delegate void HealthChanged(float health, float max);
    public event HealthChanged EventHealthChanged;
    //if(EventHealthChanged != null){EventHealthChanged.Invoke(health, maxHealth); }

    private void Start() {
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    private void Update() {
        //Is bleeding out timer
        if (isBleedingOut) {
            if(!isBeingRevived)
                timeUntilDeath -= Time.deltaTime;
            if (timeUntilDeath <= 0)
                Die();
        }
        RegenUpdate();
    }
    public float GetMaxHealth() { return maxHealth; }
    public void ChangeMaxHealth(float balance){
        maxHealth = Mathf.RoundToInt(balance * maxHealth);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    //Heals by healAmount up to maxHealth
    [Server]
    public void Heal(float healAmount) {
        float newHealth = health + healAmount;
        if (newHealth > maxHealth)
            newHealth = maxHealth;
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    public void RegenUpdate() // Called every frame update, checks if player should be healing. If so, call Heal function 
    {
        if (isBleedingOut || isDead)
            return;

        if (timeSinceHit < regenHitDelay)
        {
            timeSinceHit += Time.deltaTime;
        }
        else if (timeSinceRegen < regenInterval)
        {
            timeSinceRegen += Time.deltaTime;
        }
        else
        {
            Heal(regenAmount);
            timeSinceRegen = 0;
        }
    }
    public void ChangeRegenValues(float balance) // Called by Recovery perk, increases speed of healing
    {
        regenHitDelay *= balance;
        regenInterval *= balance;
    }
    [Command(requiresAuthority = false)]
    public void Damage(float damageAmount) {
        if (inIFrames || isBleedingOut || isDead)
            return;
        StartIFrames();

        float newHealth = health - damageAmount;
        if (newHealth <= 0) {
            GoDown();
        }
        health = newHealth;

        //Hit effect
        GameObject obj = Instantiate(hitEffectPrefab);
        obj.transform.position = transform.position;
        NetworkServer.Spawn(obj);

        // timeSinceHit resets health regeneration in RegenUpdate function
        timeSinceHit = 0; 

        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth);}

        int chance = UnityEngine.Random.Range(0,hurtsounds.Length);

        //SoundPlayer.Play(hurtsounds[chance], transform.position, volume * 1);
        AudioManager.instance.PlaySound(hurtsounds[chance]);
    }
    private void StartIFrames() {
        timer.CreateTimer(iFrameTime, EndIFrames);
        inIFrames = true;
    }
    private void EndIFrames() {
        inIFrames = false;
    }

    [Command(requiresAuthority = false)]
    public void Revive() {
        ReviveRPC();
    }
    [ClientRpc]
    private void ReviveRPC() {
        if (reviver == null || !reviver.GetComponent<PlayerHealth>().GetIsBleedingOut()) {
            revivePrompt.Deactivate();
            //SoundPlayer.Play(reviveSound, transform.position);
            AudioManager.instance.PlaySound(reviveSound);
            reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
            reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
            isBleedingOut = false;
            isBeingRevived = false;
            health = maxHealth;
            GetComponent<PlayerMovement>().EnableMovement();
            //Debug.Log("Revived!");
            if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
        }
        //    reviver.GetComponent<PlayerHealth>().OnReviveActivateRelease(gameObject);
        OnReviveActivateRelease(reviver);
        reviver = null;
    }

    [ClientRpc]
    private void GoDown() {
        reviveTrigger.EventObjEnter += OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit += OnPlayerExitReviveTrigger;
        reviveTrigger.ForceEntry();
        timeUntilDeath = bleedOutTime;
        isBleedingOut = true;
        GetComponent<PlayerMovement>().DisableMovement();
        //foreach (GameObject obj in reviveTrigger.Hits()) {
        //    Debug.Log("OBJ tag: " + obj.tag);
        //}

        foreach (GameObject revivee in reviveTrigger.Hits()){
            revivee.GetComponent<PlayerHealth>().OnReviveActivateRelease(gameObject);
            OnReviveActivateRelease(revivee);
        }        
        //Debug.Log("Bleeding out!");
    }

    [Server]
    private void Die() {
        DieRPC();
        //The player who owns this needs to call this method since they are the authority for this object
        PlayerManager.instance.OnPlayerDie(gameObject);
    }
    [ClientRpc]
    private void DieRPC() {
        reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
        isBleedingOut = false;
        isDead = true;
    }

    public void Respawn()
    {
        health = maxHealth;
        isDead = false;
        isBeingRevived = false;
        reviveTimerID = Guid.Empty;
        GetComponent<PlayerMovement>().EnableMovement();
        //GetComponent<PlayerWeaponControl>().ResetWeapons();
        //Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    public bool IsBleedingOut() {
        return isBleedingOut;
    }
    public float GetBleedOutTimeRatio() {
        return timeUntilDeath/bleedOutTime;
    }
}
