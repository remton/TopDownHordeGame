using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
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
    public void ChangeMaxHealth(float balance)
    {
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
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); timeSinceHit = 0; } // timeSinceHit resets health regeneration in RegenUpdate function
    }

    public void Revive() {
        isBleedingOut = false;
        health = maxHealth;
        GetComponent<PlayerMovement>().EnableMovement();
        Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    private void GoDown() {
        timeUntilDeath = bleedOutTime;
        isBleedingOut = true;
        GetComponent<PlayerMovement>().DisableMovement();
        Debug.Log("Bleeding out!");
    }

    private void Die() {
        isBleedingOut = false;
        isDead = true;
        PlayerManager.instance.OnPlayerDie(gameObject);
    }
    public void Respawn()
    {
        health = maxHealth;
        isDead = false;
        GetComponent<PlayerMovement>().EnableMovement();
        GetComponent<PlayerWeaponControl>().ResetWeapons();
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
