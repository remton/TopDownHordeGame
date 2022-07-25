using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Nuke : Magic
{
    public GameObject zombiePrefab;

    [Command(requiresAuthority = false)]
    protected override void PickupCMD(GameObject player) {
        base.PickupCMD(player);
        foreach (GameObject current in PlayerManager.instance.GetActiveLocalPlayers())
        {
            current.GetComponent<PlayerStats>().AddMoney(500);
        }
        foreach (GameObject current in GameObject.FindGameObjectsWithTag("Zombie"))
        {
            current.GetComponent<ZombieHealth>().DamageCMD(500);
        }
    }

    [ClientRpc]
    public override void OnPickupRPC(GameObject player) {
        base.OnPickupRPC(player);
        Destroy(gameObject);
    }
}
