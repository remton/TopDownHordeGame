using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FiredUp : Perk
{
    private float balance = .75F; // Multiplier for fireRate 
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained"); 
        player.GetComponent<PlayerWeaponControl>().fireRateMult = (balance);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerWeaponControl>().fireRateMult = (1 / balance);
    }
}

