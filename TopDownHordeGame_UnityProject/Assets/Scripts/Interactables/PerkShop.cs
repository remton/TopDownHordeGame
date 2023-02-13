using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkShop : Interactable
{
    public AudioClip purchaseSound;
    public AudioClip FailedPurchaseSound;
    public GameObject perkPrefab;
    public GameObject popupCanvas;
    public Text popupText;
    public Image popupImage;
    private int cost;
    private int baseCost;
    private bool alreadyHave = false;
    public void TryBuyPerk(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerPerkHolder perkHolder = player.GetComponent<PlayerPerkHolder>();
        alreadyHave = player.GetComponent<PlayerPerkHolder>().HavePerk(perkPrefab);
        if (playerStats.GetBank() >= cost && !alreadyHave)
        {
            AudioManager.instance.PlaySound(purchaseSound);
            playerStats.TrySpendMoney(cost);
            player.GetComponent<PlayerPerkHolder>().AddPerk(perkPrefab);
        }
        else if (playerStats.GetBank() < cost){
            AudioManager.instance.PlaySound(FailedPurchaseSound);
        //    Debug.Log("U broke lol");
        }
        else{
        //    Debug.Log("You should already have this perk.");
        }
    }


    protected override void Awake()
    {
        base.Awake();
        baseCost = perkPrefab.GetComponent<Perk>().cost;
        cost = baseCost;
        popupCanvas.SetActive(false);
    }

    public override void OnPlayerEnter(GameObject player)
    {
        base.OnPlayerEnter(player);
        if (!interactable)
            return;
        popupCanvas.SetActive(true);
        popupText.text = perkPrefab.name + "\n$" + cost;
        popupImage.sprite = perkPrefab.GetComponent<Perk>().icon;
    }
    public override void OnPlayerExit(GameObject player)
    {
        base.OnPlayerExit(player);
        popupCanvas.SetActive(false);
    }
    public override void OnInteract(GameObject player) {
        base.OnInteract(player);
        TryBuyPerk(player);
    }
    public void SaleStart(float price)
    {
        //Debug.Log("Perk Name: " + perkPrefab);
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
