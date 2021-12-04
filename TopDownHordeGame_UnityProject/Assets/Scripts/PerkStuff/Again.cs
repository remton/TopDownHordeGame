using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Again : Perk
{
    private float balance = .9F; // Multiplier for reloadTime 
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerWeaponControl>().reloadSpeedMult = balance;
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerWeaponControl>().reloadSpeedMult = (1 / balance);
    }
}

