    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class Resistance : Perk
    {

        //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
        public override void OnPerkGained(GameObject player)
        {
            Debug.Log("Perk: " + name + " gained");
            player.GetComponent<PlayerHealth>().maxHealth += 10; 
        }

        //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
        public override void OnPerkLost(GameObject player)
        {
            Debug.Log("Perk: " + name + " lost");
            player.GetComponent<PlayerHealth>().maxHealth -= 10;
        }
    }
