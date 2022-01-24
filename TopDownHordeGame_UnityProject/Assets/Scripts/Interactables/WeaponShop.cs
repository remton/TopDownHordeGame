using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : MonoBehaviour
{
    public AudioClip purchaseSound;
    public AudioClip failPurchaseSound;

    public GameObject weaponPrefab;
    public GameObject popupCanvas;
    protected int cost;
    [SerializeField] protected int baseCost;

    public void TryBuyWeapon(GameObject player) {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        if (playerStats.GetBank() >= cost) {
            playerStats.SpendMoney(cost);
            weaponControl.PickUpWeapon(weaponPrefab);
            SoundPlayer.Play(purchaseSound, transform.position);
        }
        else {
            SoundPlayer.Play(failPurchaseSound, transform.position);
            Debug.Log("u broke lol");
        }
    }

    // Hitbox and player activation set
    protected HitBoxController hitbox;

    private void Awake() {
        cost = baseCost;
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
        popupCanvas.SetActive(false);
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
    public void SaleStart(float price)
    {
        Debug.Log("Price before: " + cost);
        cost = Mathf.FloorToInt(baseCost * price);
        Debug.Log("Price now: " + cost + "\n");
    }
    public void SaleEnd()
    {
        cost = baseCost;
        Debug.Log("Price after end: " + cost);
    }

}
