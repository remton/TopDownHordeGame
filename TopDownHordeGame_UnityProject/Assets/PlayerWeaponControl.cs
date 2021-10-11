using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControl : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private int equippedIndex;

    private bool isSwapping;
    private float timeUntilSwap;
    private bool isReloading;
    private float timeUntilReload;
    private int inMag;

    public void OnSwapWeapon() {

    }
    private void Swap() {

    }

    public void OnShoot() {

    }

    public void OnReload() {

    }

    private void Update() {
        if (isSwapping) {
            if(timeUntilSwap <= 0) {
                isSwapping = false;
                Swap();
            }
        }
    }

}
