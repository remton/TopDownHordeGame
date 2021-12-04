using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkShop : MonoBehaviour
{

    public GameObject perkPrefab;
    public GameObject popupCanvas;
    private int cost;
    private bool alreadyHave = false;
    public void TryBuyPerk(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerPerkHolder perkHolder = player.GetComponent<PlayerPerkHolder>();
        alreadyHave = player.GetComponent<PlayerPerkHolder>().HavePerk(perkPrefab);
        if (playerStats.GetBank() >= cost && !alreadyHave)
        {

            playerStats.SpendMoney(cost);
            player.GetComponent<PlayerPerkHolder>().AddPerk(perkPrefab);


            Debug.Log("Your money should have been taken.");
        }
        else if (playerStats.GetBank() < cost){
            Debug.Log("You want product, I want money.");
        }
        else
        {
            Debug.Log("You should already have this perk.");
        }
    }

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake()
    {
        hitbox = GetComponent<HitBoxController>();
        cost = perkPrefab.GetComponent<Perk>().cost;
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
        popupCanvas.SetActive(false);
    }

    public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyPerk;
        popupCanvas.SetActive(true);
        popupCanvas.GetComponentInChildren<Text>().text = perkPrefab.name + "\n$" + cost;
    }
    public void OnPlayerExit(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyPerk;
        popupCanvas.SetActive(false);
    }


}
