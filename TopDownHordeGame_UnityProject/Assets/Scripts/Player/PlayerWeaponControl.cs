using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


//TODO:
// Update movement penalty when we equip a weapon

public class PlayerWeaponControl : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {
        foreach (Weapon weapon in weapons) {
            weapon.AddReserveAmmo(int.MaxValue);
            weapon.Reload();
        }
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());
    }

    [SerializeField] private List<Weapon> weapons;
    private int equippedIndex;

    private bool isSwapping;
    private float timeUntilSwap;
    private bool isReloading;
    private float timeUntilReload;
    private bool shootButtonDown;
    private bool isWaitingToShoot;
    private float timeUntilShoot;

    private int NextWeaponIndex() {
        int next = equippedIndex + 1;
        if (next >= weapons.Count)
            next = 0;
        return next;
    }

    public delegate void AmmoChanged(int mag, int reserve);
    public event AmmoChanged EventAmmoChanged;
    //Whenever ammo is changed add this code:
    //if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());


    // Swap Weapon control
    /// <summary> called when swap weapon button is pressed</summary>
    public void OnSwapWeapon(InputAction.CallbackContext context) {
        // Make sure this input is pressing down, not pulling off
        if (context.action.triggered == true)
            StartSwapWeapon();
    }
    private void StartSwapWeapon() {
        if (isReloading)
            CancelReload();
        Debug.Log("Swapping . . .");
        isSwapping = true;
        timeUntilSwap = weapons[NextWeaponIndex()].GetSwapTime();
    }
    private void SwapUpdate() {
        if (timeUntilSwap <= 0) {
            isSwapping = false;
            Swap();
        }
        timeUntilSwap -= Time.deltaTime;
    }
    private void Swap() {
        Debug.Log("Swapped!");
        equippedIndex = NextWeaponIndex();
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());
    }
    private void CancelSwap() {
        isSwapping = false;
    }
    

    // Reload weapon control
    /// <summary> called when reload button is pressed </summary>
    public void OnReload(InputAction.CallbackContext context) {
        // Make sure this input is pressing down, not pulling off
        if (context.action.triggered == true)
            StartReload();
    }
    public void StartReload() {
        if (isSwapping)
            return;
        if (weapons[equippedIndex].ReserveEmpty()) {
            StartSwapWeapon();
            return;
        }

        Debug.Log("Reloading . . .");
        isReloading = true;
        timeUntilReload = weapons[equippedIndex].GetReloadTime();
    }
    private void ReloadUpdate() {
        if(timeUntilReload <= 0) {
            Reload();
            isReloading = false;
        }
        timeUntilReload -= Time.deltaTime;
    }
    private void Reload() {
        Debug.Log("Reloaded!");
        weapons[equippedIndex].Reload();
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());
    }
    private void CancelReload() {
        isReloading = false;
    }


    // Shoot weapon control
    /// <summary> Called when shoot button state changes </summary>
    public void OnShoot(InputAction.CallbackContext context) {
        shootButtonDown = context.action.triggered;
        if (isSwapping || isReloading)
            shootButtonDown = false;

        //We are still waiting on the fire deley from the previous shot so we do nothing
        if (isWaitingToShoot) {
            return;
        }
        else {
            Shoot();
            timeUntilShoot = weapons[equippedIndex].GetFireDeley();
            isWaitingToShoot = true;
        }
    }
    private void ShootUpdate() {
        if (isWaitingToShoot) {
            if(timeUntilShoot <= 0) {
                if (shootButtonDown) {
                    Shoot();   
                    timeUntilShoot = weapons[equippedIndex].GetFireDeley();
                }
                else {
                    isWaitingToShoot = false;
                }
            }
            timeUntilShoot -= Time.deltaTime;
        }
    }
    private void Shoot() {
        if (weapons[equippedIndex].MagEmpty()) {
            //TODO: show player a reload message or auto reload
            if(!isReloading)
                StartReload();
            return;
        }
        weapons[equippedIndex].Fire(gameObject, playerMovement.GetCurrentLookDir());
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());
    }
    private void CancelShoot() {
        shootButtonDown = false;
    }

    private void Update() {
        if (isWaitingToShoot) {
            ShootUpdate();
        }
        if (isSwapping) {
            SwapUpdate();
        }
        if (isReloading) {
            ReloadUpdate();
        }
    }
}
