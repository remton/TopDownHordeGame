using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private SpriteMask mask;
    public void TurnOn() {
        mask.enabled = true;
    }
    public void TurnOff() {
        mask.enabled = false;
    }
}
