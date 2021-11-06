using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth;
    public float bleedOutTime;
    private int health;
    private float timeUntilDeath;
    private bool isBleedingOut;
    private bool isDead = false;

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
    }

    //Heals by healAmount up to maxHealth
    public void Heal(int healAmount) {
        if (isBleedingOut || isDead)
            return;

        int newHealth = health + healAmount;
        if (newHealth > maxHealth)
            newHealth = maxHealth;
        health = newHealth;
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
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
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    public void Revive() {
        isBleedingOut = false;
        health = maxHealth;
        Debug.Log("Revived!");
        if (EventHealthChanged != null) { EventHealthChanged.Invoke(health, maxHealth); }
    }

    private void GoDown() {
        timeUntilDeath = bleedOutTime;
        isBleedingOut = true;
        Debug.Log("Bleeding out!");
    }

    private void Die() {
        isBleedingOut = false;
        isDead = true;
        Debug.Log("You Died");
    }
}
