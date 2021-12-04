using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Doors : MonoBehaviour
{
    public int cost;
    public GameObject doorHolder;
    public GameObject popupCanvas;
    public List<Window> roomWindows; 

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
        popupCanvas.SetActive(false);
    }

    public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyDoor;
        popupCanvas.GetComponentInChildren<Text>().text = "$" + cost;
        popupCanvas.SetActive(true);
    }
    public void OnPlayerExit(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyDoor;
        popupCanvas.SetActive(false);
    }
    private void openDoor()
    {
        doorHolder.SetActive(false);
        RoundController.instance.ActivateWindows(roomWindows); 
    }

}
