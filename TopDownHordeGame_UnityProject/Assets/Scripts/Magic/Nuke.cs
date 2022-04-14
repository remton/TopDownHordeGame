using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Nuke : MonoBehaviour
{
    public AudioClip pickupSound;
    public int time;
    public MagicType type;
    public GameObject zombiePrefab;
    private GameObject[] zombies;
    private GameObject[] players;

    private void Awake()
    {
        GetComponent<HitBoxController>().EventObjEnter += Touch;
    }

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void Touch(GameObject player)
    {
        GetComponent<HitBoxController>().EventObjEnter -= Touch;
        Debug.Log("Power Up: " + name + " spawned");
        zombies = GameObject.FindGameObjectsWithTag("Zombie");
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject current in zombies)
        {
            current.GetComponent<ZombieHealth>().Damage(500);
        }
        foreach (GameObject current in players)
        {
            current.GetComponent<PlayerStats>().AddMoney(500);
        }
        Stop();
        AudioManager.instance.PlaySound(pickupSound, transform.position);
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop()
    {
        Debug.Log("Power Up: " + name + " lost");
        Destroy(gameObject);
    }
}
