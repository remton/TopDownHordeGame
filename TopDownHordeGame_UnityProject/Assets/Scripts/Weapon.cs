using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float reloadTime;  // time in seconds that this weapon takes to reload
    [SerializeField] private float fireDeley;   // time between shots (handled in playerWeaponControl)
    [SerializeField] private float swapTime;   // time to switch to this weapon (handled in playerWeaponControl)
    [SerializeField] private float movePenalty; // subtractive penalty to movespeed when equipped (handled in playerWeaponControl)

    [SerializeField] protected int damage;        // damage per bullet
    [SerializeField] protected int magSize;       // size of this weapons magazine
    [SerializeField] protected int reserveSize;   // max ammo that can be held with this weapon
    protected int inMag = 0; // bullets in magazine
    protected int inReserve = 0; // bullets in reserve

    public float GetReloadTime() { return reloadTime; }
    public float GetFireDeley() { return fireDeley; }
    public float GetSwapTime() { return swapTime; }
    public float GetMovePenalty() { return movePenalty; }

    /// <summary> Returns true if there is not ammo in the magazine </summary>
    public bool MagEmpty() {
        return inMag <= 0;
    }

    public bool ReserveEmpty() {
        return inReserve <= 0;
    }

    /// <summary> Reloads weapon instantly (reloadTime is handled in PlayerWeaponControl script) </summary>
    public void Reload() {
        if(inReserve >= magSize) {
            inReserve -= magSize - inMag;
            inMag = magSize;
        }
        else {
            inMag = inReserve;
            inReserve = 0;
        }
    }

    /// <summary> adds the given amount of bullets to the reserve ammo up to reserveSize </summary>
    public void AddReserveAmmo(int amount) {
        inReserve += amount;
        if (inReserve > reserveSize)
            inReserve = reserveSize;
    }

    /// <summary> Fires weapon instantly (fireDeley is handled in PlayerWeaponControl script) </summary>
    public virtual void Fire(GameObject player, Vector2 direction) {
        inMag--;
        Debug.Log(name + ": " + inMag.ToString() + " / " + inReserve.ToString());
    }
}
