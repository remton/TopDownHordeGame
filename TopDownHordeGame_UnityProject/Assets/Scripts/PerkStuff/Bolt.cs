/*
 * Increases sprinting speed of the player
 * Increases max stamina
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : Perk
{
    private float balanceSpeed = 1.1F; // Percent increase of speed 
    private float balanceStamina = 1.7F;
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerMovement>().runSpeedMultipliers.Add(balanceSpeed);
        player.GetComponent<PlayerMovement>().ChangeMaximumStamina(balanceStamina);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerMovement>().runSpeedMultipliers.Remove(balanceSpeed);
        player.GetComponent<PlayerMovement>().ChangeMaximumStamina(1 / balanceStamina);
    }
}

