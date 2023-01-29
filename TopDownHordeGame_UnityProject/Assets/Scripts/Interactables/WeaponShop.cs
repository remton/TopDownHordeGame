using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShop : Interactable
{
    public AudioClip purchaseSound;
    public AudioClip failPurchaseSound;

    public GameObject weaponPrefab;
    public GameObject popupCanvas;
    public Text popupText;
    public Image popupImage;
    protected int cost;
    [SerializeField] protected int baseCost;

    virtual public void TryBuyWeapon(GameObject player) {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerWeaponControl weaponControl = player.GetComponent<PlayerWeaponControl>();
        if (playerStats.GetBank() >= cost) {
            playerStats.TrySpendMoney(cost);
            weaponControl.PickUpWeapon(weaponPrefab);
            AudioManager.instance.PlaySound(purchaseSound);
        }
        else {
            AudioManager.instance.PlaySound(failPurchaseSound);
            //Debug.Log("u broke lol");
        }
    }

    private void Awake() {
        cost = baseCost;
        popupCanvas.SetActive(false);
    }

    public override void OnPlayerEnter(GameObject player) {
        base.OnPlayerEnter(player);
        if (!interactable)
            return;
        popupCanvas.SetActive(true);
        popupText.text = weaponPrefab.name + "\n$" + cost;
        popupImage.sprite = weaponPrefab.GetComponent<Weapon>().icon;
    }
    public override void OnPlayerExit(GameObject player) {
        base.OnPlayerExit(player);
        popupCanvas.SetActive(false);
    }
    public override void OnInteract(GameObject player) {
        base.OnInteract(player);
        TryBuyWeapon(player);
    }
    public void SaleStart(float price)
    {
        //Debug.Log("Price before: " + cost);
        cost = Mathf.FloorToInt(baseCost * price);
        //Debug.Log("Price now: " + cost + "\n");
    }
    public void SaleEnd()
    {
        cost = baseCost;
        //Debug.Log("Price after end: " + cost);
    }

}
