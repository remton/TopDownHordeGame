using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ammo : Magic
{
    private List<GameObject> players;

    //Each client refills their own ammo
    [ClientRpc]
    public override void OnPickupRPC(GameObject player)
    {
        base.OnPickupRPC(player);
        Debug.Log("Ammo Refilled!");
        players = PlayerManager.instance.GetActiveLocalPlayers(); 
        foreach(GameObject current in players)
        {
            current.GetComponent<PlayerWeaponControl>().RefillWeaponReserve();
        }
        Destroy(gameObject);
    }

}
