using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSidebar : MonoBehaviour
{
    // {var} is replaced with the variables in each update method 
    public Text HealthTxt;
    private static readonly string healthTxtDefault = "Health: {health} / {max}";

    public Text ammoTxt;
    private static readonly string ammoTxtDefault = "Ammo: {mag} / {reserve}";

    public Text bankTxt;
    private static readonly string bankTxtDefault = "Bank: ${bank}";

    public Text weaponTxt;
    private static readonly string weaponTxtDefault = "Weapon: {weapon}";

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

    public void UpdateBankTxt(int newBank) {
        string newTxt = bankTxtDefault;
        newTxt = newTxt.Replace("{bank}", newBank.ToString());
        bankTxt.text = newTxt;
    }

    public void UpdateAmmoTxt(int mag, int reserve) {
        string newTxt = ammoTxtDefault;
        newTxt = newTxt.Replace("{mag}", mag.ToString());
        newTxt = newTxt.Replace("{reserve}", reserve.ToString());
        ammoTxt.text = newTxt;
    }

    public void UpdateHealthTxt(int health, int max) {
        string newTxt = healthTxtDefault;
        newTxt = newTxt.Replace("{health}", health.ToString());
        newTxt = newTxt.Replace("{max}", max.ToString());
        HealthTxt.text = newTxt;
    }

    public void UpdateWeaponTxt(string weaponName)
    {
        string newTxt = weaponTxtDefault;
        newTxt = newTxt.Replace("{weapon}", weaponName);
        weaponTxt.text = newTxt;
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
