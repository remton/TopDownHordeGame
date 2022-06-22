using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles a popup window that should only be displayed when a player is in a trigger
public class Popup : MonoBehaviour
{
    public HitBoxController hitBox;
    public GameObject popupObj;
    private bool popupEnabled;

    public List<GameObject> players;

    private void Start() {
        hitBox.EventObjEnter += PlayerEntersTrigger;
        hitBox.EventObjExit += PlayerExitsTrigger;
        popupObj.SetActive(false);
    }
    private void OnDestroy() {
        hitBox.EventObjEnter -= PlayerEntersTrigger;
        hitBox.EventObjExit -= PlayerExitsTrigger;
    }

    private void PlayerEntersTrigger(GameObject player) {
        players.Add(player);
        if (!popupEnabled) {
            popupEnabled = true;
            popupObj.SetActive(true);
        }
    }
    private void PlayerExitsTrigger(GameObject player) {
        players.Remove(player);
        if (popupEnabled && players.Count == 0) {
            popupEnabled = false;
            popupObj.SetActive(false);
        }
    }


}
