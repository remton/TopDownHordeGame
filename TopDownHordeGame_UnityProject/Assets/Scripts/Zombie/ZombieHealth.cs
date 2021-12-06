using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    private int chance;
    public void SetMaxHealth(int newMax)
    {
        maxHealth = newMax;
        health = maxHealth;
    }

    private int maxHealth = 1;
    private int health;

    //public delegate void OnHealthChange(int newHealth);
    //public event OnHealthChange EventHealthChange;

    public bool isDead()
    {
        return health <= 0;
    }

    private void Start()
    {
        health = maxHealth;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
        //if (EventHealthChange != null) { EventHealthChange.Invoke(health); }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
        //if (EventHealthChange != null) { EventHealthChange.Invoke(health); }
    }

    private void Die()
    {
        Debug.Log(name + ": \"*dies\"");
        RoundController.instance.ZombieDies();
        chance = Random.Range(0, 1000);
        // MagicController.MagicDrop(chance) // CALL DROP FUNCTION -- UNSURE OF WHERE TO PUT IT CURRENTLY 
        Destroy(gameObject);
    }
}
