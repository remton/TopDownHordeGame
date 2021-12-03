    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class Resistance : Perk
    {

        //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
        public override void OnPerkGained(GameObject player)
        {
            int tempHealth;
            Debug.Log("Perk: " + name + " gained");
            tempHealth = player.GetComponent<PlayerHealth>().GetMaxHealth();
            tempHealth += 10;
            player.GetComponent<PlayerHealth>().ChangeMaxHealth(tempHealth);
        }

        //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
        public override void OnPerkLost(GameObject player)
        {
            Debug.Log("Perk: " + name + " lost");
            player.GetComponent<PlayerHealth>().ChangeMaxHealth(-10);
        }
    }
