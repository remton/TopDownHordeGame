using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIPlayerSidebar : MonoBehaviour
{
    public GameObject sidebarObj;
    public Text playerNameTxt;
    public static readonly string bankTxtDefault = "Bank ${bank}";
    public Text bankTxt;

    [SerializeField] private List<GameObject> perkIconObjects;

    // {var} is replaced with the variables in each update method 
    //public Text healthTxt;
    //private static readonly string healthTxtDefault = "{health} / {max}";
    //public Text ammoTxt;
    //private static readonly string ammoTxtDefault = "{mag} / {reserve}";
    //public Text weaponTxt;

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
}
