using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerRevive : NetworkBehaviour {

    public delegate void OnRevive();
    public event OnRevive EventOnRevive;

    public float reviveTime;

    private PlayerBleedout bleedout;
    private Timer timer;
    private Guid timerID;

    [SerializeField]
    private HitBoxController reviveTrigger;
    private GameObject revivee;

    private void Awake() {
        timer = GetComponent<Timer>();
        bleedout = GetComponent<PlayerBleedout>();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        //Only enable inputs and events for our local player characters
        if (GetComponent<Player>().IsLocalCharacter()) {
            GetComponent<PlayerActivate>().EventPlayerActivate += OnActionDown;
            GetComponent<PlayerActivate>().EventPlayerActivateRelease += OnActionUp;
            reviveTrigger.EventObjExit += OnLeaveReviveTrigger;
            reviveTrigger.EventObjEnter += OnEnterReviveTrigger;
        }
    }

    [Client]
    public bool CanRevive() {
        return !bleedout.isBleedingOut;
    }

    [Client]
    private void OnEnterReviveTrigger(GameObject otherPlayer) {
        Debug.Log("PlayerRevive: Entered Trigger");
        if (CanRevive() && otherPlayer.GetComponent<PlayerBleedout>().isBleedingOut) {
            otherPlayer.GetComponent<PlayerRevive>().ShowRevivePrompt(true);
        }
    }
    [Client]
    private void OnLeaveReviveTrigger(GameObject otherPlayer) {
        Debug.Log("PlayerRevive: Left Trigger");
        if (CanRevive() && otherPlayer.GetComponent<PlayerBleedout>().isBleedingOut) {
            otherPlayer.GetComponent<PlayerRevive>().ShowRevivePrompt(false);
        }
    }
    [Client]
    private void OnActionDown(GameObject player) {
        if (!CanRevive())
            return;
        List<GameObject> others = reviveTrigger.Hits();
        foreach (GameObject other in others) {
            if (other.GetComponent<PlayerBleedout>().isBleedingOut) {
                StartRevive(other);
                return;
            }
        }
    }
    [Client]
    private void OnActionUp(GameObject player) {
        StopRevive();
    }

    [Command(requiresAuthority = false)]
    private void StartRevive(GameObject newRevivee) {
        timerID = timer.CreateTimer(reviveTime, ReviveOther);
        revivee = newRevivee;
        revivee.GetComponent<PlayerBleedout>().PauseBleedout();
        revivee.GetComponent<PlayerRevive>().ShowRevivingTRPC(connectionToClient, true);
    }
    [Command(requiresAuthority = false)]
    private void StopRevive() {
        timer.KillTimer(timerID);
        if (revivee == null)
            return;
        revivee.GetComponent<PlayerRevive>().ShowRevivingTRPC(connectionToClient, false);
        revivee.GetComponent<PlayerBleedout>().UnpauseBleedout();
        revivee = null;
    }

    [Client]
    private void ShowRevivePrompt(bool b) {
       bleedout.bleedOutMeter.SetRevivePrompt(b);
    }
    [TargetRpc]
    private void ShowRevivingTRPC(NetworkConnection network, bool b) {
        bleedout.bleedOutMeter.SetReviving(b);
    }

    [Server]
    public void ReviveOther() {
        if (revivee == null)
            return;
        revivee.GetComponent<PlayerRevive>().ReviveSelf();
    }
    [Command(requiresAuthority = false)]
    public void ReviveSelfCMD() {
        ReviveSelf();
    }
    [Server]
    public void ReviveSelf() {
        if (EventOnRevive != null) { EventOnRevive.Invoke(); }
        bleedout.StopBleedout();
    }
}
