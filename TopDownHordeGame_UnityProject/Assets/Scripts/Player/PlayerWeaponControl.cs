using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Mirror;


public class PlayerWeaponControl : NetworkBehaviour {
    public bool isPaused = false;

    [SyncVar]
    public bool laserSightEnabled;

    private PlayerMovement playerMovement;
    private Timer timer;

    public float reloadSpeedMult;
    public float fireDelayMult;
    public float magMult;
    public float reserveMult;
    public GameObject starterWeaponPrefab;

    private int equippedIndex;

    private List<Weapon> weapons = new List<Weapon>();
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

    public bool NeedReticle() { return !laserSightEnabled || isReloading || isSwapping; }

    //---------------- EVENTS ----------------
    public delegate void ReloadCalled(float reloadTimeSec);
    public event ReloadCalled EventReloadCalled;
    public delegate void AmmoChanged(int mag, int reserve);
    public event AmmoChanged EventAmmoChanged;
    public delegate void WeaponChanged(string weaponName);
    public event WeaponChanged EventWeaponChanged;
    public delegate void OwnedWeaponsChanged(List<Weapon> oldWeapon, List<Weapon> newWeapons);
    public event OwnedWeaponsChanged EventOwnedWeaponsChange;
    public delegate void SwapWeaponCalled(float swapTimeSec);
    public event SwapWeaponCalled EventStartSwapWeapon;

    //Whenever ammo is changed add this code:
    //if (EventAmmoChanged != null) EventAmmoChanged.Invoke(weapons[equippedIndex].GetInMag(), weapons[equippedIndex].GetInReserve());

    //--- Networking Stuff ---
    [SyncVar(hook = nameof(OnEquippedWeaponChange))] 
    private Weapon equippedWeapon;
    public Weapon GetEquippedWeapon() { return equippedWeapon; }
    public List<Weapon> GetWeapons() { return weapons; }


    private void OnEquippedWeaponChange(Weapon oldWeapon, Weapon newWeapon) {
        if(oldWeapon != null)
            DeactivateWeapon(oldWeapon);
        if(newWeapon != null)
            ActivateWeapon(newWeapon);
        UpdateVisuals();
    }

    [Command(requiresAuthority = false)]
    private void EquipWeapon(Weapon newEquippedWeapon) {
        equippedWeapon = newEquippedWeapon;
    }
    [Client]
    private void SetEquippedIndex(int newIndex) {
        equippedIndex = newIndex;
        if(weapons.Count > equippedIndex)
            EquipWeapon(weapons[equippedIndex]);
    }

    [Client]
    public void PickUpWeapon(GameObject weaponPrefab) {
        //Debug.Log("Picking up weapon " + weaponPrefab.name);

        if (isReloading)
            CancelReload();
        if (isSwapping)
            CancelSwap();
        if (isWaitingToShoot)
            CancelShoot();
        int index = MyNetworkManager.instance.GetPrefabIndex(weaponPrefab);
        CreateWeapon(index);
    }
    [Command(requiresAuthority = false)]
    private void CreateWeapon(int prefabIndex) {
        GameObject weaponPrefab = MyNetworkManager.instance.GetPrefab(prefabIndex);
        //Debug.Log("Creating weapon: " + weaponPrefab.name);
        GameObject weaponObj = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(weaponObj);
        weaponObj.GetComponent<Weapon>().SetOwner(GetComponent<Player>());
        weaponObj.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
    }
    [Command]
    private void DeleteWeapon(Weapon weapon) {
        NetworkServer.Destroy(weapon.gameObject);
    }
    [Client]
    public void AddWeapon(Weapon weapon) {
        //Debug.Log("Adding weapon . . .");

        List<Weapon> oldWeapons = weapons;
        weapon.AddReserveAmmo(Mathf.RoundToInt(weapon.GetReserveSize() * reserveMult));
        if (maxWeapons > weapons.Count) {
            //Add a new weapon to list
            weapons.Add(weapon);
            SetEquippedIndex(weapons.Count - 1);
        }
        else {
            //Replace the current weapon
            if (weapons.Count > 0) {
                if (weapons[equippedIndex] != null) {
                    DeleteWeapon(weapons[equippedIndex]);
                }
                weapons[equippedIndex] = weapon;
                SetEquippedIndex(equippedIndex);
            }
        }
        UpdateVisuals();
        if(EventOwnedWeaponsChange != null) { EventOwnedWeaponsChange.Invoke(oldWeapons, weapons); }
    }

    private void DeactivateWeapon(Weapon weapon) {
        playerMovement.runSpeedMultipliers.Remove(weapon.GetMoveMult());
        playerMovement.walkSpeedMultipliers.Remove(weapon.GetMoveMult());
        weapon.spriteControl.DeactivateSprite();
    }
    private void ActivateWeapon(Weapon weapon) {
        weapon.spriteControl.ActivateSprite();
        playerMovement.runSpeedMultipliers.Add(weapon.GetMoveMult());
        playerMovement.walkSpeedMultipliers.Add(weapon.GetMoveMult());
    }

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        timer = GetComponent<Timer>();
        GetComponent<Player>().EventDoInputChange += DoInputChange;
    }

    private void Start() {
        //Debug.Log("START player weapon control. local? " + GetComponent<Player>().isLocalPlayer + " Netid:" + netId);

        if (GetComponent<Player>().IsLocalCharacter()) {
            ResetWeapons();
        }
    }
    private void Update() {
        if (equippedWeapon != null) {
            equippedWeapon.spriteControl.DirectionChanged(GetComponent<PlayerMovement>().GetCurrentLookDir());
        }
    }

    //Used to Change Weapon count for perks or whatever else in the future
    [Client]
    public void SetWeaponCount(int newCount) {
        for (int i = newCount; i < weapons.Count; i++) {
            if (weapons[i] != null) {
                DeleteWeapon(weapons[i]);
            }
            weapons.RemoveAt(i);
        }
        maxWeapons = newCount;
        if(equippedIndex >= maxWeapons) {
            SetEquippedIndex(NextWeaponIndex());
        }
    }

    private int NextWeaponIndex() {
        int next = equippedIndex + 1;
        if (next >= weapons.Count)
            next = 0;
        return next;
    }

    // ------------- LASER CONTROL -------------
    [Client]
    public void OnLaserButton(InputAction.CallbackContext context) {
        if (!doInput)
            return;

        if (context.action.triggered == true) {
            laserSightEnabled = !laserSightEnabled;
            equippedWeapon.spriteControl.SetLaser(laserSightEnabled);
        }
    }

    // ------------ SWAPPING ------------

    // Swap Weapon control
    /// <summary> called when swap weapon button is pressed</summary>
    public void OnSwapWeapon(InputAction.CallbackContext context) {
        if (!doInput)
            return;
        if (isPaused)
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
        if (EventStartSwapWeapon != null) EventStartSwapWeapon.Invoke(timeUntilSwap);
    }
    private void Swap() {
        isSwapping = false;
        equippedWeapon.PlaySwapSound();
        SetEquippedIndex(NextWeaponIndex());
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
        if (!doInput)
            return;
        if (isPaused)
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
        if (!doInput)
            return;
        if (isPaused)
            return;

        //We dont have a weapon
        if (weapons.Count <= 0)
            return;

        shootButtonDown = context.action.triggered;
        if (shootButtonDown && repeatingReload && !weapons[equippedIndex].MagEmpty())
            repeatingReload = false;

        if (isSwapping || isReloading)
            shootButtonDown = false;

        //We are still waiting on the fire deley from the previous shot so we do nothing
        if (isWaitingToShoot)
            return;

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
            if (!isReloading) {
                StartReload();
                if (GetComponent<PlayerPerkHolder>().HavePerk(electricPrefab)) {
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
        //We dont have a weapon
        if (weapons.Count <= 0 || equippedWeapon == null)
            return;

        if (EventAmmoChanged != null) EventAmmoChanged.Invoke(Mathf.RoundToInt(equippedWeapon.GetInMag()), Mathf.RoundToInt(equippedWeapon.GetInReserve()));
        if (EventWeaponChanged != null) EventWeaponChanged.Invoke(equippedWeapon.GetWeaponName());
        equippedWeapon.spriteControl.SetLaser(laserSightEnabled);
    }

    public void RefillWeaponReserve()
    {
        foreach(Weapon current in weapons)
        {
            current.AddReserveAmmo(Mathf.RoundToInt(current.GetReserveSize() * reserveMult - current.GetInReserve())); //Changed AddReserveAmmo function. The commented out portion may be unnecessary. 
            UpdateVisuals();
        }
    }
//    public void KillDamage(int killDamage)
//    {
////        Debug.Log("Damage should be changed");
//        foreach (Weapon current in weapons)
//            current.SetDamage(killDamage);
//        UpdateVisuals();
//    }
//    public void ResetKillDamage()
//    {
////        Debug.Log("Damage should be reset");
//        foreach (Weapon current in weapons)
//            current.ResetDamage();
//        UpdateVisuals();
//    }
    public void ResetWeapons()
    {
        SetWeaponCount(0);
        SetWeaponCount(2);
        PickUpWeapon(starterWeaponPrefab);
    }

    private bool doInput = true;
    private void DoInputChange(bool b) {
        doInput = b;
    }
}
