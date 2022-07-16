using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Carpenter : Magic
{
    //The server boards the windows
    [Command(requiresAuthority = false)]
    protected override void PickupCMD(GameObject player)
    {
        base.PickupCMD(player);
        foreach (Window current in RoundController.instance.GetActiveWindows())
        {
            current.GetComponent<Window>().FullRepair();
        }
        foreach (GameObject current in PlayerManager.instance.GetActiveLocalPlayers())
        {
            current.GetComponent<PlayerStats>().AddMoney(1200);
        }
    }
    [ClientRpc]
    public override void OnPickupRPC(GameObject player) {
        base.OnPickupRPC(player);
        Destroy(gameObject);
    }
}
