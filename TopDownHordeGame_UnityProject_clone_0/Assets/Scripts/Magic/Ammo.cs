using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public AudioClip pickupSound;

    public int time;
    public MagicType type;
    private List<GameObject> players;

    private void Awake()
    {
        GetComponent<HitBoxController>().EventObjEnter += Touch;
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter-= Touch;
        Debug.Log("Power Up: " + name + " spawned");
        players = PlayerManager.instance.GetActiveLocalPlayers(); 
        foreach(GameObject current in players)
        {
            current.GetComponent<PlayerWeaponControl>().RefillWeaponReserve();
        }
        AudioManager.instance.PlaySound(pickupSound);
        Stop();
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop()
    {
        Debug.Log("Power Up: " + name + " lost");
        Destroy(gameObject); 
    }
}
