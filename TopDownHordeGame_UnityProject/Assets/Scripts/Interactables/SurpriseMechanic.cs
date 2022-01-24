using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurpriseMechanic : WeaponShop
{
    //public GameObject[] weaponPrefabList;
    public List<RandomChoice> weaponChoices;
    //private int cost;
    //public int baseCost;

    new public void TryBuyWeapon(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        if (playerStats.GetBank() >= cost)
        {
            SoundPlayer.Play(purchaseSound, transform.position);
            playerStats.SpendMoney(cost);
            weaponControl.PickUpWeapon(RandomChoice.ChooseRandom(weaponChoices));
        }
        else
        {
            SoundPlayer.Play(failPurchaseSound, transform.position);
            Debug.Log("Need something. Money, perhaps.");
        }
    }

    private void Awake()
    {
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
        popupCanvas.SetActive(false);
        cost = baseCost;
    }

    new public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyWeapon;
        popupCanvas.SetActive(true);
        popupCanvas.GetComponentInChildren<Text>().text = "Surprise Mechanic" + "\n$" + cost; // "Myster Box"
    }
}
