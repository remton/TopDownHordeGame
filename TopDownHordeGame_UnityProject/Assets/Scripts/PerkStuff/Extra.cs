using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Extra : Perk
{
    private float balance = 1.6F; // Multiplier for Reserve ammo  
    //This is where the perk activates. This changes the reserve size for the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerWeaponControl>().reserveMult = (balance);
    }

    //This is where the perk deactivates. 
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerWeaponControl>().reserveMult = (1 / balance);
    }
}

