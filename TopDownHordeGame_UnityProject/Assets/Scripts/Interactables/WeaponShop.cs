using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public GameObject weaponPrefab;
    public GameObject popupCanvas;
    public int cost;

    public void TryBuyWeapon(GameObject player) {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        if (playerStats.GetBank() >= cost) {
            playerStats.SpendMoney(cost);
            weaponControl.PickUpWeapon(weaponPrefab);
        }
        else {
            Debug.Log("u broke lol");
        }
    }

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake() {
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
    }

    public void OnPlayerEnter(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyWeapon;
        popupCanvas.SetActive(true);
        popupCanvas.GetComponentInChildren<Text>().text = weaponPrefab.name + "\n$" + cost;
    }
    public void OnPlayerExit(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyWeapon;
        popupCanvas.SetActive(false);
    }


}
