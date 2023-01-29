using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Interactable : NetworkBehaviour {
    [SerializeField] private HitBoxController trigger;

    public bool interactable;

    //Events
    public delegate void OnEnter(GameObject player);
    public delegate void OnExit(GameObject player);
    public delegate void OnInteracted(GameObject player);
    public event OnEnter EventOnEnter;
    public event OnExit EventOnExit;
    public event OnInteracted EventOnInteract;

    protected virtual void Awake() {
        trigger.EventObjEnter += OnEnterHitbox;
        trigger.EventObjExit += OnExitHitbox;
    }

    private void OnEnterHitbox(GameObject player) {
        //Debug.Log("Player Enter Interactable Hitbox");
        player.GetComponent<PlayerActivate>().EventPlayerActivate += PlayerAction;
        OnPlayerEnter(player);
    }
    private void OnExitHitbox(GameObject player) {
        //Debug.Log("Player Exit Interactable Hitbox");
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= PlayerAction;
        OnPlayerExit(player);
    }
    private void PlayerAction(GameObject player) {
        if (interactable) {
            OnInteract(player);
        }
    }

    //virtual overrides

    public virtual void OnPlayerEnter(GameObject player) {
        //Debug.Log("Player Enter Interactable");
        if (EventOnEnter != null) { EventOnEnter.Invoke(player); }
    }

    public virtual void OnPlayerExit(GameObject player) {
        //Debug.Log("Player Exit Interactable");
        if (EventOnExit != null) { EventOnExit.Invoke(player); }
    }

    public virtual void OnInteract(GameObject player) {
        //Debug.Log("Player Interact Interactable");
        if (EventOnInteract != null) { EventOnInteract.Invoke(player); }
    }
}

