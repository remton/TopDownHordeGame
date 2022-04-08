using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


//TODO:
// Update movement penalty when we equip a weapon

public class PlayerWeaponControl : MonoBehaviour
{
    public bool isDisabled = false;

    private bool laserSightEnabled;

    private PlayerMovement playerMovement;
    private Timer timer;

    public float reloadSpeedMult;
    public float fireDelayMult;
    public float magMult;
    public float reserveMult;
    public GameObject starterWeaponPrefab;

    private int equippedIndex;
    public List<Weapon> weapons;
    [SerializeField] private int maxWeapons;

    // Electric perk vars
    public float electricRadius;
    public int electricDamage;
    public GameObject electricPrefab;

    //Internal booleans
    private bool isSwapping;
    private bool isReloading;
    private bool shootButtonDown;
    private bool isWaitingToShoot;


    //---------------- EVENTS ----------------
    public delegate void ReloadCalled(float reloadTimeSec);
    public event ReloadCalled EventReloadCalled;
    public delegate void AmmoChanged(int mag, int reserve);
    public event AmmoChanged EventAmmoChanged;
    public delegate void WeaponChanged(string weaponName);
    public event WeaponChanged EventWeaponChanged;
    public delegate void SwapWeaponCalled(float swapTimeSec);
    public event SwapWeaponCalled EventSwapCalled;
    //Whenever ammo is changed add this code:
    //if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());


    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        timer = GetComponent<Timer>();
    }
    private void Start() {
        for (int i = 0; i < weapons.Count; i++) {
            weapons[i].AddReserveAmmo(Mathf.RoundToInt(weapons[i].GetReserveSize() * reserveMult));
            weapons[i].Reload();
            weapons[equippedIndex].spriteControl.ActivateSprite();
            laserSightEnabled = true;
            weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
        }
        EventAmmoChanged.Invoke(Mathf.RoundToInt(weapons[equippedIndex].GetInMag()), weapons[equippedIndex].GetInReserve());
        EventWeaponChanged.Invoke(weapons[equippedIndex].GetWeaponName());
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
    }
    private void Update() {
        weapons[equippedIndex].spriteControl.UpdateDirection(playerMovement.GetCurrentLookDir());
    }

    //Used to Change Weapon count for perks or whatever else in the future
    public void SetWeaponCount(int newCount) {
        for (int i = 0; i < maxWeapons - newCount; i++) {
            if (weapons[i] != null) {
                Destroy(weapons[equippedIndex].gameObject);
            }
            weapons.RemoveAt(i);
        }
        maxWeapons = newCount;
    }

    private int NextWeaponIndex() {
        int next = equippedIndex + 1;
        if (next >= weapons.Count)
            next = 0;
        return next;
    }

    // ------------- LASER CONTROL -------------
    public void OnLaserButton(InputAction.CallbackContext context) {
        if(context.action.triggered == true) {
            laserSightEnabled = !laserSightEnabled;
            weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
        }
    }

    // ------------ SWAPPING ------------

    // Swap Weapon control
    /// <summary> called when swap weapon button is pressed</summary>
    public void OnSwapWeapon(InputAction.CallbackContext context) {
        if (isDisabled)
            return;
        // Make sure this input is pressing down, not pulling off
        if (context.action.triggered == true)
            StartSwapWeapon();
    }
    private void StartSwapWeapon() {
        if (isSwapping)
            return;
        if (isReloading)
            CancelReload();
        CancelShoot();
        Debug.Log("Swapping . . .");
        isSwapping = true;
        float timeUntilSwap = weapons[NextWeaponIndex()].GetSwapTime();
        timer.CreateTimer(timeUntilSwap, Swap);
        if (EventSwapCalled != null)EventSwapCalled.Invoke(timeUntilSwap);
    }
    private void Swap() {
        isSwapping = false;
        weapons[equippedIndex].PlaySwapSound();
        Debug.Log("Swapped!");
        playerMovement.runSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Remove(weapons[equippedIndex].GetMoveMult());
        weapons[equippedIndex].spriteControl.DeactivateSprite();
        equippedIndex = NextWeaponIndex();
        weapons[equippedIndex].spriteControl.ActivateSprite();
        weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
        weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        UpdateVisuals();
    }
    private void CancelSwap() {
        isSwapping = false;
    }

    // ----------- RELOADING --------------
    bool repeatingReload = true;
    // Reload weapon control
    /// <summary> called when reload button is pressed </summary>
    public void OnReload(InputAction.CallbackContext context) {
        if (isDisabled)
            return;
        // Make sure this input is pressing down, not pulling off
        if ((context.action.triggered == true) && (weapons[equippedIndex].GetInMag() < Mathf.RoundToInt(weapons[equippedIndex].GetMagSize() * magMult)))
            StartReload();
    }
    public void StartReload() {
        if (isSwapping || isReloading)
            return;
        CancelShoot();
        repeatingReload = true;

        if (weapons[equippedIndex].ReserveEmpty()) {
            StartSwapWeapon();
            return;
        }

        isReloading = true;
        float timeUntilReload = weapons[equippedIndex].GetReloadTime();
        timer.CreateTimer(timeUntilReload, Reload);
        if (EventReloadCalled != null) EventReloadCalled.Invoke(timeUntilReload);
    }
    private void Reload() {
        weapons[equippedIndex].PlayReloadSound();
        int magSize = Mathf.RoundToInt(weapons[equippedIndex].GetMagSize() * magMult);
        weapons[equippedIndex].Reload(magSize);
        UpdateVisuals();
        //we need to reload again if... 
        if (repeatingReload // We havent canceled the reload
            && weapons[equippedIndex].GetInReserve() > 0 // We still have ammo in reserve
            && weapons[equippedIndex].GetInMag() < magSize) { //We havent filled our mag yet
            isReloading = false;
            StartReload();
        }
        else {
            repeatingReload = false; 
            isReloading = false;
        }
    }
    private void CancelReload() {
        isReloading = false;
    }

    //--------------------- SHOOTING ---------------------

    // Shoot weapon control
    /// <summary> Called when shoot button state changes </summary>
    public void OnShoot(InputAction.CallbackContext context) {
        if (isDisabled)
            return;
        shootButtonDown = context.action.triggered;
        if (shootButtonDown && repeatingReload && !weapons[equippedIndex].MagEmpty())
            repeatingReload = false;

        if (isSwapping || isReloading)
            shootButtonDown = false;

        //We are still waiting on the fire deley from the previous shot so we do nothing
        if (isWaitingToShoot) {
            return;
        }
        else if (shootButtonDown) {
            Shoot();
            if (weapons[equippedIndex].IsAutomatic())
                timer.CreateTimer(weapons[equippedIndex].GetFireDeley(), AutomaticShoot);
            else
                timer.CreateTimer(weapons[equippedIndex].GetFireDeley(), EndShooting);
            isWaitingToShoot = true;
        }
    }
    private void EndShooting() {
        isWaitingToShoot = false;
    }
    //Method for automatic weapons
    private void AutomaticShoot() {
        if (shootButtonDown) {
            //We need to shoot and start a new timer
            Shoot();
            timer.CreateTimer(weapons[equippedIndex].GetFireDeley(), AutomaticShoot);
        }
        else {
            EndShooting();
        }
    }
    //Shoot Once
    private void Shoot() {
        if (weapons[equippedIndex].MagEmpty()) {
            repeatingReload = true;
            if (!isReloading){
                StartReload();
                if (GetComponent<PlayerPerkHolder>().HavePerk(electricPrefab)){
                    // Check if the player has the Electric reload perk. 
                    // To do: Fix this to make it call the Electric perk's damage function. 
                    GetComponent<PlayerPerkHolder>().CallElectricDamage(electricPrefab);  
                }
            }
            return;
        }
        else {
            weapons[equippedIndex].Fire(gameObject, playerMovement.GetCurrentLookDir());
        }
        UpdateVisuals();
    }
    private void CancelShoot() {
        //if (weapons[equippedIndex].IsAutomatic() && !isReloading && !isSwapping)
        //    weapons[equippedIndex].StopSound();
        shootButtonDown = false;
    }

    // ------ Other Misc Methods ------

    public void UpdateVisuals() {
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
        weaponObj.transform.position = gameObject.transform.position;
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        weapon.AddReserveAmmo(Mathf.RoundToInt(weapon.GetReserveSize() * reserveMult));
        if(maxWeapons > weapons.Count) {
            weapons.Add(weapon);
            weapons[equippedIndex].spriteControl.DeactivateSprite();
            equippedIndex = weapons.Count - 1;
            weapons[equippedIndex].spriteControl.ActivateSprite();
            weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
        }
        else {
            if (weapons[equippedIndex] != null) {
                Destroy(weapons[equippedIndex].gameObject);
            }
            weapons[equippedIndex] = weapon;
            weapons[equippedIndex].spriteControl.ActivateSprite();
            weapons[equippedIndex].spriteControl.SetLaser(laserSightEnabled);
            laserSightEnabled = !laserSightEnabled;
        }
        UpdateVisuals();
        playerMovement.runSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapons[equippedIndex].GetMoveMult());
    }

    public void RefillWeaponReserve()
    {
        foreach(Weapon current in weapons)
        {
            current.AddReserveAmmo(Mathf.RoundToInt(current.GetReserveSize() * reserveMult - current.GetInReserve())); //Changed AddReserveAmmo function. The commented out portion may be unnecessary. 
            UpdateVisuals();
        }
    }
    public void KillDamage(int killDamage)
    {
//        Debug.Log("Damage should be changed");
        foreach (Weapon current in weapons)
            current.SetKillDamage(killDamage);
        UpdateVisuals();
    }
    public void ResetKillDamage()
    {
//        Debug.Log("Damage should be reset");
        foreach (Weapon current in weapons)
            current.ResetDamage();
        UpdateVisuals();
    }
    public void ResetWeapons()
    {
        SetWeaponCount(0);
        SetWeaponCount(2);
        PickUpWeapon(starterWeaponPrefab);
    }
}
