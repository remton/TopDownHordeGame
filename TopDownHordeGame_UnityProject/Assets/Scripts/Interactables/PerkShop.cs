using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkShop : MonoBehaviour
{
    public GameObject perkPrefab;
    public int cost;
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
            Debug.Log("You should already have the perk.");
        }
    }

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake()
    {
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
    }

    public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyPerk;
    }
    public void OnPlayerExit(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyPerk;
    }


}
