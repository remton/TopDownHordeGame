using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Perk : MonoBehaviour
{
    public int cost;

    public virtual void OnPerkGained(GameObject player) {
        Debug.Log("Perk: " + name + " gained");
    }

    public virtual void OnPerkLost(GameObject player) {
        Debug.Log("Perk: " + name + " lost");
    }
}
