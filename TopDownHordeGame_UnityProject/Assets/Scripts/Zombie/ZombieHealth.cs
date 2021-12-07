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
        // MagicController.MagicDrop(chance) 
        Vector3 myLocation = transform.position;
        MagicController.instance.MagicDrop(myLocation);
        Destroy(gameObject);

    }
}
