using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int health;

    private void Start() {
        health = maxHealth;
    }

    public void Damage(int amount) {
        Debug.Log(name + ": hit for " + amount);
        health -= amount;
        if (health <= 0)
            Die();
    }

    public void Heal(int amount) {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
    }

    private void Die() {
        Debug.Log(name + ": \"*dies\"");
        Destroy(gameObject);
    }
}
