using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Kill : Magic
{
    public int time;
    private float balanceTime = 20.0F; 
    private List<GameObject> players;
    private Timer timer;

    protected override void Awake() {
        base.Awake();
        timer = GetComponent<Timer>();
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    [ClientRpc]
    public override void OnPickupRPC(GameObject player)
    {
        base.OnPickupRPC(player);
        GetComponent<TimedDestroy>().Cancel();
        players = PlayerManager.instance.GetActiveLocalPlayers();
        foreach (GameObject current in players)
        {
            current.GetComponent<PlayerWeaponControl>().KillDamage(5000);
        }
        transform.position = holdingRoom;
        timer.CreateTimer(balanceTime, Stop);
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public void Stop()
    {
        foreach (GameObject current in players)
        {
            current.GetComponent<PlayerWeaponControl>().ResetKillDamage();
        }
        Destroy(gameObject);
    }
}
