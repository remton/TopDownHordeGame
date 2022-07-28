using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Sale : Magic
{
    private static Sale activeSale = null;

    [SerializeField] private float balanceCost = .2F; // multpilier to normal costs
    private GameObject[] perks;
    private GameObject[] weapons;

    protected override void Awake()
    {
        base.Awake();
        timer = GetComponent<Timer>();
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    [ClientRpc]
    public override void OnPickupRPC(GameObject player)
    {
        GetComponent<TimedDestroy>().Cancel(); 
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks){
            current.GetComponent<PerkShop>().SaleStart(balanceCost);
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons){
            current.GetComponent<WeaponShop>().SaleStart(balanceCost);
        }
        base.OnPickupRPC(player);
    }

    protected override void StartTimer() {
        if (activeSale == null) {
            activeSale = this;
            transform.position = holdingRoom;
            timerID = timer.CreateTimer(time, OnTimerEnd);
            MagicController.instance.CreateTimer(this, timerID);
        }
        else {
            activeSale.ResetTimer();
            Destroy(gameObject);
        }
    }

    protected override void OnTimerEnd() {
        base.OnTimerEnd();
        foreach (GameObject current in perks) {
            current.GetComponent<PerkShop>().SaleEnd();
        }
        foreach (GameObject current in weapons) {
            current.GetComponent<WeaponShop>().SaleEnd();
        }
        activeSale = null;
        Destroy(gameObject);
    }
    private void ResetTimer() {
        timer.SetTimer(timerID, time, OnTimerEnd);
    }
}
