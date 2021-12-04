using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Recovery : Perk
{
    private float balance = .8f;
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerHealth>().ChangeRegenValues(balance);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerHealth>().ChangeRegenValues(1/balance);
    }
}
