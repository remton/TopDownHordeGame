/*
 * Player reagians health faster
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Recovery : Perk
{
    private float balance = .7f;
    private float balanceStamina = 1.2f;
    //This is where the perk activates. This changes the regen values of the player.
    public override void OnPerkGained(GameObject player)
    {
        Debug.Log("Perk: " + name + " gained");
        player.GetComponent<PlayerHealth>().ChangeRegenValues(balance);
        player.GetComponent<PlayerMovement>().ChangeRegenValues(balanceStamina);
    }

    //This is where the perk deactivates. This changes the regen values of the player.
    public override void OnPerkLost(GameObject player)
    {
        Debug.Log("Perk: " + name + " lost");
        player.GetComponent<PlayerHealth>().ChangeRegenValues(1/balance);
        player.GetComponent<PlayerMovement>().ChangeRegenValues(1/balanceStamina);
    }
}
