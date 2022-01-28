using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Sale : MonoBehaviour
{
    private Timer timer;
    public AudioClip pickupSound;
    public MagicType type;
    [SerializeField] private float balanceCost = .2F; // multpilier to normal costs
    [SerializeField] private float balanceTime = 30.0F;
    private GameObject[] perks;
    private GameObject[] weapons;
    private Vector3 holdingRoom;

    private void Awake()
    {
        timer = GetComponent<Timer>();
        GetComponent<HitBoxController>().EventObjEnter += Touch;
        holdingRoom = transform.position * 10;
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter -= Touch;
        GetComponent<TimedDestroy>().Cancel(); 
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks)
        {
            current.GetComponent<PerkShop>().SaleStart(balanceCost);
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons)
        {
            current.GetComponent<WeaponShop>().SaleStart(balanceCost);
        }
        SoundPlayer.Play(pickupSound, transform.position);
        transform.position = holdingRoom;
        timer.CreateTimer(balanceTime, Stop);
    }

    //This is where the perk deactivates. 
    public virtual void Stop()
    {
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks)
        {
            current.GetComponent<PerkShop>().SaleEnd();
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons)
        {
            current.GetComponent<WeaponShop>().SaleEnd();
        }
        MagicController.instance.selling = false;
        Destroy(gameObject);
    }
}
