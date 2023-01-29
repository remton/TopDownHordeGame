using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Doors : Interactable
{
    public int cost;
    public GameObject doorHolder;
    public GameObject popupCanvas;
    public List<ZombieSpawn> roomSpawns;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip failedBuySound;
    public void TryBuyDoor(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats.GetBank() >= cost){
            playerStats.TrySpendMoney(cost);
            OpenDoor();
            //SoundPlayer.Play(openSound, transform.position);
            AudioManager.instance.PlaySound(openSound);
        }
        else {
            //SoundPlayer.Play(failedBuySound, transform.position);
            AudioManager.instance.PlaySound(failedBuySound);
            Debug.Log("Get a job.");
        }
    }


    private void Awake(){
        doorHolder.SetActive(true);
        popupCanvas.SetActive(false);
    }

    public override void OnPlayerEnter(GameObject player){
        base.OnPlayerEnter(player);
        if (!interactable)
            return;
        popupCanvas.GetComponentInChildren<Text>().text = "$" + cost;
        popupCanvas.SetActive(true);
    }
    public override void OnPlayerExit(GameObject player){
        base.OnPlayerExit(player);
        popupCanvas.SetActive(false);
    }
    public override void OnInteract(GameObject player) {
        base.OnInteract(player);
        TryBuyDoor(player);
    }

    [Client]
    private void OpenDoor()
    {
        OpenDoorCMD();
    }
    [Command(requiresAuthority = false)]
    private void OpenDoorCMD() {
        RoundController.instance.ActivateSpawns(roomSpawns);
        OpenDoorRPC();
    }
    [ClientRpc]
    private void OpenDoorRPC() {
        Destroy(gameObject);
    }
}
