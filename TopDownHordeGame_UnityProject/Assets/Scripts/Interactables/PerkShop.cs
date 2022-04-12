using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkShop : MonoBehaviour
{
    public AudioClip purchaseSound;
    public AudioClip FailedPurchaseSound;
    public GameObject perkPrefab;
    public GameObject popupCanvas;
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
            //SoundPlayer.Play(purchaseSound,transform.position);
            AudioManager.instance.PlaySound(purchaseSound, transform.position);
            playerStats.SpendMoney(cost);
            player.GetComponent<PlayerPerkHolder>().AddPerk(perkPrefab);
        }
        else if (playerStats.GetBank() < cost){
            //SoundPlayer.Play(FailedPurchaseSound, transform.position);
            AudioManager.instance.PlaySound(FailedPurchaseSound, transform.position);
        //    Debug.Log("U broke lol");
        }
        else{
        //    Debug.Log("You should already have this perk.");
        }
    }

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake()
    {
        baseCost = perkPrefab.GetComponent<Perk>().cost;
        cost = baseCost;
        hitbox = GetComponent<HitBoxController>();
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
    public void SaleStart(float price)
    {
        Debug.Log("Perk Name: " + perkPrefab);
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
