using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkShop : MonoBehaviour
{
    public GameObject perkPrefab;
    public int cost;
    public void TryBuyPerk(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerPerkHolder perkHolder = player.GetComponent<PlayerPerkHolder>();
        if (playerStats.GetBank() >= cost)
        {
            playerStats.SpendMoney(cost);
            player.GetComponent<PlayerPerkHolder>().AddPerk(perkPrefab);
            player.GetComponent<PlayerHealth>().Damage(0);
            Debug.Log("Your money should have been taken.");
        }
        else
        {
            Debug.Log("You want product, I want money.");
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
