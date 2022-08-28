using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRevive : NetworkBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private BleedOutMeter bleedOutMeter;
    [SerializeField] private HitBoxController reviveTrigger;
    [SerializeField] private AudioClip reviveSound;
    [SerializeField] private float bleedOutTime;
    [SerializeField] private float reviveTime;

    public bool isBleedingOut { get; internal set; }
    private bool hasRevivePrompt;
    private bool isBeingRevived;
    private GameObject reviver;
    private Timer timer;
    private System.Guid bleedOutTimer;
    private System.Guid reviveTimer;

    public bool GetIsBleedingOut() { return isBleedingOut; }
    [Client]
    public float GetBleedOutTimeRatio() { 
        return timer.TimeLeft(bleedOutTimer) / bleedOutTime; 
    }


    #region Server

    [Server]
    public void StartBleedingOut() {
        StartBleedingOutRPC();
        bleedOutTimer = timer.CreateTimer(bleedOutTime, Death);
    }
    [Command(requiresAuthority = false)]
    public void ReviveCMD() {
        playerHealth.Revive();
        ReviveRPC();
    }
    [Server]
    public void Death() {
        isBleedingOut = false;
        playerHealth.Kill();
    }

    #endregion


    #region Client

    [ClientRpc]
    public void StartBleedingOutRPC() {
        if (!isServer) {
            isBleedingOut = true;
            GetComponent<PlayerMovement>().DisableMovement();
            bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
            bleedOutTimer = timer.CreateTimer(bleedOutTime, null);
            reviveTrigger.EventObjEnter += OnPlayerEnterReviveTrigger;
            reviveTrigger.EventObjExit += OnPlayerExitReviveTrigger;
        }
    }
    [Client]
    private void OnPlayerEnterReviveTrigger(GameObject otherPlayer) {
        hasRevivePrompt = true;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate += ReviveStarted;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease += ReviveStopped;
    }
    [Client]
    private void OnPlayerExitReviveTrigger(GameObject otherPlayer) {
        hasRevivePrompt = false;
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivate -= ReviveStarted;
        otherPlayer.GetComponent<PlayerActivate>().EventPlayerActivateRelease -= ReviveStopped;
        ReviveStopped(otherPlayer);
    }
    [Client]
    private void ReviveStarted(GameObject reviver) {
        if(isBleedingOut && !isBeingRevived && !reviver.GetComponent<PlayerRevive>().isBleedingOut) {
            this.reviver = reviver;
            reviveTimer = timer.CreateTimer(reviveTime, Revive);
        }
    }
    [Client]
    public void Revive() {
        this.reviver = null;
        ReviveCMD();
    }
    private void ReviveStopped(GameObject reviver) {
        if(this.reviver == reviver) {
            timer.KillTimer(reviveTimer);
            this.reviver = null;
        }
    }

    [ClientRpc]
    private void ReviveRPC() {
        GetComponent<PlayerMovement>().EnableMovement();
        isBleedingOut = false;
        isBeingRevived = false;
        AudioManager.instance.PlaySound(reviveSound);
        bleedOutMeter.UpdateValues(isBleedingOut, hasRevivePrompt, isBeingRevived);
        reviveTrigger.EventObjEnter -= OnPlayerEnterReviveTrigger;
        reviveTrigger.EventObjExit -= OnPlayerExitReviveTrigger;
    }
    #endregion

}
