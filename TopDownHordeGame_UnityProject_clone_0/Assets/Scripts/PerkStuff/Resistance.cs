    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class Resistance : Perk
    {
    private float balance = 1.5f; 

        //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
        public override void OnPerkGained(GameObject player)
        { 
            Debug.Log("Perk: " + name + " gained");
            player.GetComponent<PlayerHealth>().ChangeMaxHealth(balance);
        }

        //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
        public override void OnPerkLost(GameObject player)
        {
            Debug.Log("Perk: " + name + " lost");
            player.GetComponent<PlayerHealth>().ChangeMaxHealth(1/balance);
        }
    }
