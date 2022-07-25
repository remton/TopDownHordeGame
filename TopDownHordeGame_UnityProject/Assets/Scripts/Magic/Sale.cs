using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Sale : Magic
{
    private Timer timer;
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
        base.OnPickupRPC(player);
        GetComponent<TimedDestroy>().Cancel(); 
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks){
            current.GetComponent<PerkShop>().SaleStart(balanceCost);
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons){
            current.GetComponent<WeaponShop>().SaleStart(balanceCost);
        }
        transform.position = holdingRoom;
        timer.CreateTimer(time, Stop);
    }

    //This is where the perk deactivates. 
    public void Stop()
    {
        foreach (GameObject current in perks){
            current.GetComponent<PerkShop>().SaleEnd();
        }
        foreach (GameObject current in weapons) {
            current.GetComponent<WeaponShop>().SaleEnd();
        }
        Destroy(gameObject);
    }
}
