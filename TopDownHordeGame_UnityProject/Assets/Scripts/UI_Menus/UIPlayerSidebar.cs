using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSidebar : MonoBehaviour
{
    public Text playerNameTxt;

    // {var} is replaced with the variables in each update method 
    public Text healthTxt;
    private static readonly string healthTxtDefault = "{health} / {max}";
    public Text ammoTxt;
    private static readonly string ammoTxtDefault = "{mag} / {reserve}";

    public Text bankTxt;
    public Text weaponTxt;

    public Image weaponImg;
    public List<Image> perkImgs;

    [SerializeField] private GameObject player;

    private void Awake() {
        if (player != null)
            AttachToPlayer(player);
    }

    //Adds this objects methods to the events linked to the given player.
    //This automatically detaches the current player if there is one
    public void AttachToPlayer(GameObject newPlayer) {
        DetachCurrentPlayer();
        player = newPlayer;
        ChangePlayerName(player.GetComponent<PlayerStats>().playerName);
        player.GetComponent<PlayerWeaponControl>().EventAmmoChanged += UpdateAmmoTxt;
        player.GetComponent<PlayerHealth>().EventHealthChanged += UpdateHealthTxt;
        player.GetComponent<PlayerStats>().EventBankChange += UpdateBankTxt;
        player.GetComponent<PlayerWeaponControl>().EventWeaponChanged += UpdateWeaponTxt;
    }
    //Removes this objects methods from the events linked to its player
    public void DetachCurrentPlayer() {
        if(player != null) {
            player.GetComponent<PlayerWeaponControl>().EventAmmoChanged -= UpdateAmmoTxt;
            player.GetComponent<PlayerHealth>().EventHealthChanged -= UpdateHealthTxt;
            player.GetComponent<PlayerStats>().EventBankChange -= UpdateBankTxt;
            player.GetComponent<PlayerWeaponControl>().EventWeaponChanged -= UpdateWeaponTxt;
        }
    }
    public void ChangePlayerName(string newName) {
        playerNameTxt.text = newName;
        playerNameTxt.resizeTextForBestFit = true;
    }

    public void UpdateBankTxt(int newBank) {
        //string newTxt = bankTxtDefault;
        //newTxt = newTxt.Replace("{bank}", newBank.ToString());
        bankTxt.text = newBank.ToString();
        bankTxt.resizeTextForBestFit = true;
    }

    public void UpdateAmmoTxt(int mag, int reserve) {
        string newTxt = ammoTxtDefault;
        newTxt = newTxt.Replace("{mag}", mag.ToString());
        newTxt = newTxt.Replace("{reserve}", reserve.ToString());
        ammoTxt.text = newTxt;
        ammoTxt.resizeTextForBestFit = true;
    }

    public void UpdateHealthTxt(int health, int max) {
        string newTxt = healthTxtDefault;
        newTxt = newTxt.Replace("{health}", health.ToString());
        newTxt = newTxt.Replace("{max}", max.ToString());
        healthTxt.text = newTxt;
        healthTxt.resizeTextForBestFit = true;
    }

    public void UpdateWeaponTxt(string weaponName)
    {
        weaponTxt.text = weaponName;
        weaponTxt.resizeTextForBestFit = true;
    }

    public void UpdateWeaponImg(Sprite img) {
        //TODO: Implement weapon images
        Debug.Log("Weapon image updated . . .");
    }

    public void UpdatePerkImages(List<Sprite> imgs) {
        //TODO: Implement perk images
        Debug.Log("Perk images updated . . .");
    }
}
