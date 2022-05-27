using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitBoxController))]
public class Pickup : MonoBehaviour
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

    public void OnPlayerEnter(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += Collect;
    }
    public void OnPlayerExit(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= Collect;
    }

    public void Collect(GameObject player) {
        AudioManager.instance.PlaySound(collectSound);
        if (EventOnCollect != null)
            EventOnCollect.Invoke();
        else
            Debug.LogWarning("Pickup Collected without calling anything!!");
        Destroy(gameObject);
    }
}
