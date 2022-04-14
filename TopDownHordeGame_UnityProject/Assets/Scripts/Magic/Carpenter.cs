using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carpenter : MonoBehaviour
{
    public AudioClip pickupSound;
    public MagicType type;
    private void Awake(){
        GetComponent<HitBoxController>().EventObjEnter += Touch;
        GameObject.FindGameObjectsWithTag("Player");
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter -= Touch;
        Debug.Log("Power Up: " + name + " spawned");
        foreach (Window current in RoundController.instance.GetActiveWindows())
        {
            current.GetComponent<Window>().FullRepair();
        }
        foreach (GameObject current in PlayerManager.instance.GetActivePlayers())
        {
            current.GetComponent<PlayerStats>().AddMoney(1200);
        }
        AudioManager.instance.PlaySound(pickupSound, transform.position);
        Stop();
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop()
    {
        Debug.Log("Power Up: " + name + " lost");
        Destroy(gameObject);
    }
}
