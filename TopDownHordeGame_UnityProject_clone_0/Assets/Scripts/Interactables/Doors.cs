using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Doors : MonoBehaviour
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

    // Hitbox and player activation set
    private HitBoxController hitbox;

    private void Awake()
    {
        doorHolder.SetActive(true);
        hitbox = GetComponent<HitBoxController>();
        hitbox.EventObjEnter += OnPlayerEnter;
        hitbox.EventObjExit += OnPlayerExit;
        popupCanvas.SetActive(false);
    }

    public void OnPlayerEnter(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TryBuyDoor;
        popupCanvas.GetComponentInChildren<Text>().text = "$" + cost;
        popupCanvas.SetActive(true);
    }
    public void OnPlayerExit(GameObject player)
    {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TryBuyDoor;
        popupCanvas.SetActive(false);
    }
    private void OpenDoor()
    {
        RoundController.instance.ActivateSpawns(roomSpawns);
        Destroy(gameObject);
    }

}
