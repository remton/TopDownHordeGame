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
    public Sprite icon;
    protected HitBoxController trigger;
    protected bool pickedUp;

    protected virtual void Awake() {
        trigger = GetComponent<HitBoxController>();
        trigger.EventObjEnter += Pickup;
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

    /// <summary>
    /// Called on every client when a player picks this up
    /// </summary>
    /// <param name="player">The player who picked this up</param>
    [ClientRpc]
    public virtual void OnPickupRPC(GameObject player)
    {
        //Debug.Log("Power Up: " + name + " picked up");
        AudioManager.instance.PlaySound(pickupSound);
        if (useTimer)
            MagicController.instance.CreateTimer(this);
    }

}
