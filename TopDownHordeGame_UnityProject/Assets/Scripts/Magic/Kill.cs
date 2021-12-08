using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Kill : MonoBehaviour
{
    public int time;
    public MagicType type;
    private float balanceTime = 5.0F;
    private float currentTime = 0F;
    private bool touched; 
    private List<GameObject> players;
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
        Debug.Log("Power Up: " + name + " activated");
        players = PlayerManager.instance.GetActivePlayers();
        foreach (GameObject current in players)
        {
            Debug.Log("In player loop");
            current.GetComponent<PlayerWeaponControl>().KillDamage(5000);
        }
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
//        Debug.Log("In stall");

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
        players = PlayerManager.instance.GetActivePlayers();
        foreach (GameObject current in players)
        {
            Debug.Log("In the reset loop");
            current.GetComponent<PlayerWeaponControl>().ResetKillDamage();
        }
        Debug.Log("Power Up: " + name + " lost");
        Destroy(gameObject);
    }
}
