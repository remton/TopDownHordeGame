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

    public float reloadSpeedMult;
    public float fireRateMult;
    public float magMult;
    public float reserveMult;

    public float electricRadius;
    public int electricDamage;
    public GameObject electricPrefab;


    [SerializeField] private int maxWeapons;
    public void SetWeaponCount(int newCount) {
        for (int i = 0; i < maxWeapons - newCount; i++) {
            if (weapons[i] != null) {
                Destroy(weapons[equippedIndex].gameObject);
            }
            weapons.RemoveAt(i);
        }
        maxWeapons = newCount;
    }

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {
        foreach (Weapon weapon in weapons) {
            Debug.Log("ADDING AMMO TO: " + weapon.name);
            weapon.AddReserveAmmo(Mathf.RoundToInt(weapon.GetReserveSize() * reserveMult));
            weapon.Reload();
        }
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(Mathf.RoundToInt(weapons[equippedIndex].GetInMag()), weapons[equippedIndex].GetInReserve());
        if (EventWeaponChanged != null) EventWeaponChanged.Invoke(weapons[equippedIndex].GetWeaponName());
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
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
    public delegate void WeaponChanged(string weaponName);
    public event WeaponChanged EventWeaponChanged;
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
        playerMovement.runSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult());
        equippedIndex = NextWeaponIndex();
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        UpdateVisuals();
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
        CancelShoot();

        if (weapons[equippedIndex].ReserveEmpty()) {
            StartSwapWeapon();
            return;
        }

        Debug.Log("Reloading . . .");
        isReloading = true;
        timeUntilReload = weapons[equippedIndex].GetReloadTime() * reloadSpeedMult;
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
        weapons[equippedIndex].Reload(Mathf.RoundToInt(weapons[equippedIndex].GetMagSize() * magMult));
        UpdateVisuals();
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
        else if (shootButtonDown) {
            Shoot();
            timeUntilShoot = weapons[equippedIndex].GetFireDeley() * fireRateMult;
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
            if (!isReloading)
            {
                StartReload();
                if (GetComponent<PlayerPerkHolder>().HavePerk(electricPrefab))  // Check if the player has the Electric reload perk. 
                {
                    GetComponent<PlayerPerkHolder>().CallElectricDamage(electricPrefab);  // To do: Fix this to make it call the Electric perk's damage function. 
                }
            }
            return;
        }
        weapons[equippedIndex].Fire(gameObject, playerMovement.GetCurrentLookDir());
        UpdateVisuals();
    }
    private void CancelShoot() {
        shootButtonDown = false;
    }

    private void UpdateVisuals() {
        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(Mathf.RoundToInt(weapons[equippedIndex].GetInMag()), Mathf.RoundToInt(weapons[equippedIndex].GetInReserve()));
        if (EventWeaponChanged != null) EventWeaponChanged.Invoke(weapons[equippedIndex].GetWeaponName()); 
    }

    public void PickUpWeapon(GameObject weaponPrefab) {
        playerMovement.runSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult()); 
        if (isReloading)
            CancelReload();
        if (isSwapping)
            CancelSwap();
        if (isWaitingToShoot)
            CancelShoot();
        Debug.Log("Picked up: " + weaponPrefab.name);
        GameObject weaponObj = Instantiate(weaponPrefab, transform);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        weapon.AddReserveAmmo(Mathf.RoundToInt(weapon.GetReserveSize() * reserveMult));
        if(maxWeapons > weapons.Count) {
            weapons.Add(weapon);
            equippedIndex = weapons.Count - 1;
        }
        else {
            if (weapons[equippedIndex] != null) {
                Destroy(weapons[equippedIndex].gameObject);
            }
            weapons[equippedIndex] = weapon;
        }
        UpdateVisuals();
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
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
    public void RefillWeaponReserve()
    {
        int i = 0;
        Weapon weapon = weapons[0];
        for (i = 0; i < weapons.Count; i++)
        weapon.AddReserveAmmo(Mathf.RoundToInt(weapon.GetReserveSize() * reserveMult - weapon.GetInReserve()));
        UpdateVisuals();
    }
    public void KillDamage(int killDamage)
    {
        Debug.Log("Damage should be changed"); 
        int i = 0;
        Weapon weapon = weapons[0];
        for (i = 0; i < weapons.Count; i++)
            weapon.SetKillDamage(killDamage);
        UpdateVisuals();
    }
    public void ResetKillDamage()
    {
        Debug.Log("Damage should be changed");
        int i = 0;
        Weapon weapon = weapons[0];
        for (i = 0; i < weapons.Count; i++)
            weapon.ResetDamage();
        UpdateVisuals();
    }
}
