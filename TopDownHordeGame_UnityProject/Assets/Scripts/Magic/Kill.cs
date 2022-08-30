using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Kill : Magic
{
    static Kill activeKill = null;
    private static List<Weapon> effectedWeapons = new List<Weapon>();

    public float damageMult;
    private List<GameObject> players;

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    [ClientRpc]
    public override void OnPickupRPC(GameObject player) 
    {
        GetComponent<TimedDestroy>().Cancel();
        players = PlayerManager.instance.GetActiveLocalPlayers();
        foreach (GameObject current in players)
        {
            List<Weapon> weapons = current.GetComponent<PlayerWeaponControl>().GetWeapons();
            foreach (Weapon weapon in weapons) {
                if (!effectedWeapons.Contains(weapon)) {
                    weapon.SetDamage(weapon.GetDamage() * damageMult);
                    effectedWeapons.Add(weapon);
                }
            }
        }
        base.OnPickupRPC(player);
    }
    //This starts a timer after pickup on all clients
    protected override void StartTimer() {
        if (activeKill == null) {
            GetComponent<TimedDestroy>().Cancel();
            activeKill = this;
            transform.position = holdingRoom;
            timerID = timer.CreateTimer(time, OnTimerEnd);
            MagicController.instance.CreateTimer(this, timerID);
        }
        else if(activeKill != this){
            activeKill.ResetTimer();
            Destroy(gameObject);
        }
    }

    protected override void OnTimerEnd() {
        base.OnTimerEnd();
        foreach (Weapon weapon in effectedWeapons) {
            weapon.ResetDamage();
        }
        effectedWeapons.Clear();
        activeKill = null;
        Destroy(gameObject, 1f);
    }
    private void ResetTimer() {
        timer.SetTimer(timerID, time, OnTimerEnd);
    }
}
