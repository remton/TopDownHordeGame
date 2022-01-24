using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Sale : MonoBehaviour
{
    public AudioClip pickupSound;
    public int time;
    public MagicType type;
    private float balanceCost = .2F;
    private float balanceTime = 30.0F;
    private float currentTime = 0F;
    private bool touched;
    private GameObject[] perks;
    private GameObject[] weapons;
    private Vector3 holdingRoom;

    private void Awake()
    {
        GetComponent<HitBoxController>().EventObjEnter += Touch;
        holdingRoom = transform.position * 10;
        touched = false;
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter -= Touch;
        GetComponent<TimedDestroyActivate>().isActivated = true; 
        Debug.Log("Power Up: " + name + " activated");
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks)
        {
            Debug.Log("In perk loop");
            current.GetComponent<PerkShop>().SaleStart(balanceCost);
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons)
        {
            Debug.Log("In weapon loop");
            current.GetComponent<WeaponShop>().SaleStart(balanceCost);
        }
        SoundPlayer.Play(pickupSound, transform.position);
        Debug.Log("Moving");
        transform.position = holdingRoom;
        Debug.Log("Moved \n Calling Stall");
        touched = true;
        Debug.Log("Stall called");
    }

    private void Update()
    {
        if (touched)
        {
            StallForDuration();
        }
    }
    private void StallForDuration()
    {
    //    Debug.Log("In stall");

        if (currentTime >= balanceTime)
        {
            currentTime = 0F;
            Debug.Log("Done stalling");
            Stop();
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop()
    {
        perks = GameObject.FindGameObjectsWithTag("PerkShop");
        foreach (GameObject current in perks)
        {
            Debug.Log("In the perk reset loop");
            current.GetComponent<PerkShop>().SaleEnd();
        }
        weapons = GameObject.FindGameObjectsWithTag("WeaponShop");
        foreach (GameObject current in weapons)
        {
            Debug.Log("In the weapon reset loop");
            current.GetComponent<WeaponShop>().SaleEnd();
        }
        Debug.Log("Power Up: " + name + " lost");
        MagicController.instance.selling = false;
        Debug.Log("Sale being destroyed in the Stop() method.");
        Destroy(gameObject);
    }
}
