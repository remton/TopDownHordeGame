using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bolt : Perk
{
    private float balance = 1.1F; // Percent increase of speed 
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerMovement>().runSpeedMultipliers.Add(balance);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerMovement>().runSpeedMultipliers.Remove(balance);
    }
}

