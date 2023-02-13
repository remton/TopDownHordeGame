using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerBleedout : NetworkBehaviour
{
    public float bleedoutTime;

    [SyncVar]
    public float bleedoutRatio;

    private Timer timer;
    private Guid timerID;

    public BleedOutMeter bleedOutMeter; 

    public bool isBleedingOut { get; internal set; }

    public delegate void OnBleedout();
    public event OnBleedout EventOnBleedoutEnd;

    private void Awake() {
        timer = GetComponent<Timer>();
    }
    private void Update() {
        if (isServer && isBleedingOut)
            BleedoutUpdate();
    }

    [Server]
    public void BleedoutUpdate() {
        bleedoutRatio = timer.TimeLeft(timerID) / bleedoutTime;
    }

    [Server]
    public void StartBleedout() {
        timerID = timer.CreateTimer(bleedoutTime, BleedoutEnd);
        StartBleedoutRPC();
    }
    [ClientRpc]
    private void StartBleedoutRPC() {
        isBleedingOut = true;
        bleedOutMeter.SetBleeingOut(true);
        GetComponent<PlayerMovement>().DisableMovement();
    }

    [Server]
    public void StopBleedout() {
        timer.KillTimer(timerID);
        StopBleedoutRpc();
    }
    [ClientRpc]
    private void StopBleedoutRpc() {
        isBleedingOut = false;
        bleedOutMeter.SetBleeingOut(false);
        GetComponent<PlayerMovement>().EnableMovement();
    }

    [Server]
    public void PauseBleedout() {
        timer.PauseTimer(timerID);
    }
    [Server]
    public void UnpauseBleedout() {
        timer.UnpauseTimer(timerID);
    }

    [Server]
    public void BleedoutEnd() {
        StopBleedout();
        BleedoutEndRPC();
    }
    [ClientRpc]
    public void BleedoutEndRPC() {
        bleedOutMeter.SetBleeingOut(false);
        if (EventOnBleedoutEnd != null) { EventOnBleedoutEnd.Invoke(); }
    }
}
