using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(HitBoxController))]
public class Pickup : NetworkBehaviour
{
    public AudioClip collectSound;

    public delegate void OnCollect();
    public event OnCollect EventOnCollect;

    private HitBoxController hitBox;
    private void Awake() {
        hitBox = GetComponent<HitBoxController>();
        hitBox.EventObjEnter += OnPlayerEnter;
        hitBox.EventObjExit += OnPlayerExit;
    }

    public void Activate(bool b) {
        gameObject.SetActive(b);
    }

    public void OnPlayerEnter(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += Collect;
    }
    public void OnPlayerExit(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= Collect;
    }

    public void Collect(GameObject player) {
        if (NetworkClient.active) {
            CollectCMD();
        }
        else if(NetworkServer.active) {
            CollectRPC();
        }
        else {
            Debug.LogWarning("Client and Server both inactive");
        }
    }
    [Command(requiresAuthority = false)]
    private void CollectCMD() {
        CollectRPC();
    }
    [ClientRpc]
    private void CollectRPC() {
        AudioManager.instance.PlaySound(collectSound);
        if (EventOnCollect != null)
            EventOnCollect.Invoke();
        else
            Debug.LogWarning("Pickup Collected without calling anything!!");
        Destroy(gameObject);
    }
}
