using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public List<SpriteMask> masks;
    public void TurnOn() {
        foreach (var item in masks) {
            item.enabled = true;
        }
    }
    public void TurnOff() {
        foreach (var item in masks) {
            item.enabled = false;
        }
    }
}
