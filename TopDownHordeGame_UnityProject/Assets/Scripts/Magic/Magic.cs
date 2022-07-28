using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum MagicType
{
    Ammo, Carpenter, Kill, Nuke, Sale
}

[RequireComponent(typeof(HitBoxController))]
public abstract class Magic : NetworkBehaviour
{
    public static readonly Vector3 holdingRoom = new Vector3(1000, 1000, 1000);

    public MagicType type;
    public AudioClip pickupSound;

    public bool useTimer;
    public float time;
    public System.Guid timerID;
    public delegate void TimerEnd();
    public event TimerEnd EventTimerEnd;

    public Sprite icon;
    protected HitBoxController trigger;
    protected bool pickedUp;
    protected Timer timer;

    protected virtual void Awake() {
        trigger = GetComponent<HitBoxController>();
        trigger.EventObjEnter += Pickup;
        timer = GetComponent<Timer>();
    }

    /// <summary>
    /// Called on the client that picks this up
    /// </summary>
    /// <param name="player">The player who picked this up</param>
    [Client]
    protected virtual void Pickup(GameObject player) {
        trigger.EventObjEnter -= Pickup;
        PickupCMD(player);
    }

    /// <summary>
    /// Called on the server when a client picks this up
    /// </summary>
    /// <param name="player">The player who picked this up</param>
    [Command(requiresAuthority = false)]
    protected virtual void PickupCMD(GameObject player) {
        if (pickedUp)
            return;
        pickedUp = true;
        OnPickupRPC(player);
    }

    /// <summary>  Called on every client when a player picks this up </summary>
    /// <param name="player">The player who picked this up</param>
    [ClientRpc]
    public virtual void OnPickupRPC(GameObject player)
    {
        AudioManager.instance.PlaySound(pickupSound);
        if (useTimer)
            StartTimer();
    }

    protected virtual void StartTimer() {
        timerID = timer.CreateTimer(time, OnTimerEnd);
        MagicController.instance.CreateTimer(this, timerID);
    }
    protected virtual void OnTimerEnd() {
        if (EventTimerEnd != null) { EventTimerEnd.Invoke(); }
    }

}
