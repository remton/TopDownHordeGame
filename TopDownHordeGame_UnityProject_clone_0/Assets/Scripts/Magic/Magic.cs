using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MagicType
{
    Ammo, Carpenter, Kill, Nuke, Sale
}

public class Magic : MonoBehaviour
{
    public int time; 
    public MagicType type;

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.

    public virtual void Touch(GameObject player)
    {
        Debug.Log("Power Up: " + name + " spawned");
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void Stop(GameObject player)
    {
        Debug.Log("Power Up: " + name + " lost");
    }
}