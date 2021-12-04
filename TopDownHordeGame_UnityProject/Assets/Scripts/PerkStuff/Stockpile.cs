using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stockpile : Perk
{
    private int balance = 1; // Number of extra guns 
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        balance += 2; // Add the base two weapons to the balance value 
        player.GetComponent<PlayerWeaponControl>().SetWeaponCount(balance);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerWeaponControl>().SetWeaponCount(2);
    }
}

