using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PerkType
{
    Again, Bolt, Resistance, Recovery
}

public class Perk : MonoBehaviour
{
    public int cost;
    public PerkType type;

    //This is where the perk activates. Maybe it changes a stat value, maybe it subsribes to an event.
    public virtual void OnPerkGained(GameObject player) {
        Debug.Log("Perk: " + name + " gained");
    }

    //This is where the perk deactivates. Maybe it changes a stat value, maybe it unsibscribes from an event.
    public virtual void OnPerkLost(GameObject player) {
        Debug.Log("Perk: " + name + " lost");
    }
}
