using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSidebar : MonoBehaviour
{
    public GameObject sidebarObj;
    public Text playerNameTxt;
    public static readonly string bankTxtDefault = "Bank ${bank}";
    public Text bankTxt;

    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> perkIconObjects;

    // {var} is replaced with the variables in each update method 
    //public Text healthTxt;
    //private static readonly string healthTxtDefault = "{health} / {max}";
    //public Text ammoTxt;
    //private static readonly string ammoTxtDefault = "{mag} / {reserve}";
    //public Text weaponTxt;

    private void Awake() {
        if (player != null)
            AttachToPlayer(player);
    }

    //Adds this objects methods to the events linked to the given player.
    //This automatically detaches the current player if there is one
    public void AttachToPlayer(GameObject newPlayer) {
        DetachCurrentPlayer();
        player = newPlayer;
        SetPlayerName(player.GetComponent<PlayerStats>().playerName);
        player.GetComponent<PlayerStats>().EventBankChange += UpdateBankTxt;
        player.GetComponent<PlayerPerkHolder>().EventPerkChanged += UpdatePerkImages;

        UpdateBankTxt(player.GetComponent<PlayerStats>().GetBank());
        UpdatePerkImages(player.GetComponent<PlayerPerkHolder>().GetPerks());

        //player.GetComponent<PlayerWeaponControl>().EventAmmoChanged += UpdateAmmoTxt;
        //player.GetComponent<PlayerHealth>().EventHealthChanged += UpdateHealthTxt;
        //player.GetComponent<PlayerWeaponControl>().EventWeaponChanged += UpdateWeaponTxt;
    }
    //Removes this objects methods from the events linked to its player
    public void DetachCurrentPlayer() {
        if(player != null) {
            player.GetComponent<PlayerStats>().EventBankChange -= UpdateBankTxt;
            player.GetComponent<PlayerPerkHolder>().EventPerkChanged -= UpdatePerkImages;
            //player.GetComponent<PlayerWeaponControl>().EventAmmoChanged -= UpdateAmmoTxt;
            //player.GetComponent<PlayerHealth>().EventHealthChanged -= UpdateHealthTxt;
            //player.GetComponent<PlayerWeaponControl>().EventWeaponChanged -= UpdateWeaponTxt;
        }
    }

    public void Activate(bool isActive) {
        sidebarObj.SetActive(isActive);
    }

    public void SetPlayerName(string newName) {
        playerNameTxt.text = newName;
        playerNameTxt.resizeTextForBestFit = true;
    }

    public void UpdatePerkImages(List<Perk> perks) {
        for (int i = 0; i < perkIconObjects.Count; i++) {
            if (i < perks.Count) {
                perkIconObjects[i].SetActive(true);
                perkIconObjects[i].GetComponent<Image>().sprite = perks[i].icon;
            }
            else {
                perkIconObjects[i].SetActive(false);
            }
        }
    }

    public void UpdateBankTxt(int newBank) {
        string newTxt = bankTxtDefault;
        newTxt = newTxt.Replace("{bank}", newBank.ToString());
        bankTxt.text = newTxt;
        bankTxt.resizeTextForBestFit = true;
    }

    //public void UpdateAmmoTxt(int mag, int reserve) {
    //    string magStr = mag.ToString();
    //    string reserveStr = reserve.ToString();
    //    if (mag < 0)
    //        magStr = "inf";
    //    if (reserve < 0)
    //        reserveStr = "inf";

    //    string newTxt = ammoTxtDefault;
    //    newTxt = newTxt.Replace("{mag}", magStr);
    //    newTxt = newTxt.Replace("{reserve}", reserveStr);
    //    ammoTxt.text = newTxt;
    //    ammoTxt.resizeTextForBestFit = true;
    //}

/*    public void UpdateHealthTxt(int health, int max) {
        string newTxt = healthTxtDefault;
        newTxt = newTxt.Replace("{health}", health.ToString());
        newTxt = newTxt.Replace("{max}", max.ToString());
        healthTxt.text = newTxt;
        healthTxt.resizeTextForBestFit = true;
    } */

    //public void UpdateWeaponTxt(string weaponName)
    //{
    //    weaponTxt.text = weaponName;
    //    weaponTxt.resizeTextForBestFit = true;
    //}
}
