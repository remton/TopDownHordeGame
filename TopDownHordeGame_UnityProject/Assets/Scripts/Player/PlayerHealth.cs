using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    private Timer timer; 
    private void Awake() {
        timer = GetComponent<Timer>();
    }
    [SerializeField] private float reviveTime;
    [SerializeField] private HitBoxController reviveTrigger;
    private int reviveTimerID = -1;
    private void OnPlayerEnterReviveTrigger(GameObject otherPlayer) {
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate += OnReviveActivateDown;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease += OnReviveActivateRelease;
    }
    private void OnPlayerExitReviveTrigger(GameObject otherPlayer) {
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate -= OnReviveActivateDown;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease -= OnReviveActivateRelease;
        if (reviveTimerID != -1)
            timer.KillTimer(reviveTimerID);
    }
    private void OnReviveActivateDown(GameObject otherPlayer) {
        if (isBleedingOut && !otherPlayer.GetComponent<PlayerHealth>().isBleedingOut) {
            Debug.Log("Player start revive");
            if (reviveTimerID != -1) {
                timer.KillTimer(reviveTimerID);
                reviveTimerID = -1;
            }
            reviveTimerID = timer.CreateTimer(reviveTime, Revive);
        }
    }
    private void OnReviveActivateRelease(GameObject otherPlayer) {
        if(isBleedingOut && !otherPlayer.GetComponent<PlayerHealth>().isBleedingOut && reviveTimerID != -1) {
            Debug.Log("player end revive");
            timer.KillTimer(reviveTimerID);
            reviveTimerID = -1;
        }
    }

    [SerializeField] private int maxHealth;
    public float bleedOutTime;
    private int health;
    private float timeUntilDeath;
    private bool isBleedingOut;
    private bool isDead = false;
    private int regenAmount = 1;
    private float regenHitDelay = 10; 
    private float regenInterval = 4;
    private float timeSinceHit;
    private float timeSinceRegen;
    public GameObject hitEffectObj;
    public bool GetIsDead() { return isDead; }

    public delegate void HealthChanged(int health, int max);
    public event HealthChanged EventHealthChanged;
    //if(EventHealthChanged != null){EventHealthChanged.Invoke(health, maxHealth); }

    private void Start() {
        health = maxHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    private void Update() {
        //Is bleeding out timer
        if (isBleedingOut) {
            timeUntilDeath -= Time.deltaTime;
            if (timeUntilDeath <= 0)
                Die();
        }
        RegenUpdate();
    }
    public int GetMaxHealth() { return maxHealth; }
    public void ChangeMaxHealth(float balance){
        maxHealth = Mathf.RoundToInt(balance * maxHealth);
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    //Heals by healAmount up to maxHealth
    public void Heal(int healAmount) {
        int newHealth = health + healAmount;
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

    public void Damage(int damageAmount) {
        if (isBleedingOut || isDead)
            return;

        int newHealth = health - damageAmount;
        if (newHealth < 1) {
            GoDown();
            newHealth = 0;
        }
        health = newHealth;

        //Hit effect
        GameObject obj = Instantiate(hitEffectObj);
        obj.transform.position = transform.position;

        // timeSinceHit resets health regeneration in RegenUpdate function
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); timeSinceHit = 0; } 
    }

    public void Revive() {
        reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
        isBleedingOut = false;
        health = maxHealth;
        GetComponent<PlayerMovement>().EnableMovement();
        Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    private void GoDown() {
        reviveTrigger.EventObjEnter += OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit += OnPlayerExitReviveTrigger;
        reviveTrigger.ForceEntry();
        timeUntilDeath = bleedOutTime;
        isBleedingOut = true;
        GetComponent<PlayerMovement>().DisableMovement();
        Debug.Log("Bleeding out!");
    }

    private void Die() {
        reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
        isBleedingOut = false;
        isDead = true;
        PlayerManager.instance.OnPlayerDie(gameObject);
    }
    public void Respawn()
    {
        health = maxHealth;
        isDead = false;
        GetComponent<PlayerMovement>().EnableMovement();
//        GetComponent<PlayerWeaponControl>().ResetWeapons();
        Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }
    public bool IsBleedingOut() {
        return isBleedingOut;
    }
    public float GetBleedOutTimeRatio() {
        return timeUntilDeath/bleedOutTime;
    }
}
