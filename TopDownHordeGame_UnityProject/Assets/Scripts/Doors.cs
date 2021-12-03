using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public int cost;
    public GameObject doorHolder;

    public void TryBuyDoor(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats.GetBank() >= cost)
        {
            playerStats.SpendMoney(cost);
            openDoor();
        }
        else
        {
            Debug.Log("Get a job.");
        }
    }

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake()
    {
        doorHolder.SetActive(true);
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
    }

    public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyDoor;
    }
    public void OnPlayerExit(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyDoor;
    }
    private void openDoor()
    {
        doorHolder.SetActive(false);
    }

}
