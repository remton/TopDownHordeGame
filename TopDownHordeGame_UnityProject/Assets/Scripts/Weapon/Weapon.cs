using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private string weaponName;     // Weapon name for display
    [SerializeField] private float baseDamage;        // Used to reset damage for the magic that makes players intantly kill zombies
    [SerializeField] protected int penatration;     // number of zombies able to be hit by one bullet. (Should be at least 1)
    [SerializeField] protected float fireDeley;     // time between shots (handled in playerWeaponControl)
    [SerializeField] protected int magSize;         // size of this weapons magazine
    [SerializeField] protected int reserveSize;     // max ammo that can be held with this weapon
    [SerializeField] private float reloadTime;      // time in seconds that this weapon takes to reload
    [SerializeField] protected int reloadAmount;    // amount of bullets to put in mag on reload (used in weapons like the RemTon870)
    [SerializeField] private float swapTime;        // time to switch to this weapon (handled in playerWeaponControl)
    [SerializeField] private float movePenalty;     // subtractive penalty to movespeed when equipped (handled in playerWeaponControl)
    [SerializeField] protected bool isAutomatic;    // True if this weapon is automatic
    [SerializeField] protected bool infiniteReserve;// Does this weapon get infinite reserve
    [SerializeField] public AudioClip reloadSound;  //sound played on reload
    [SerializeField] public AudioClip swapSound;    //sound played when swapping to this weapon
    [SerializeField] public AudioClip shootSound;   //sound of gunshot
    [SerializeField] protected float shakeIntensity; //Intensity of screen shake
    [HideInInspector]public WeaponSpriteController spriteControl; //Controls the sprite for the weapon

    protected FireEffectController effectController; //Controller for bullet trail and fire sound

    protected float damage;       // damage per bullet
    protected int inMag = 0;    // bullets in magazine
    protected int inReserve = 0;// bullets in reserve

    protected GameObject owner;

    /// <summary> Called whenever the this weapon is fired </summary>
    public event WeaponFired EventWeaponFired;
    public delegate void WeaponFired(GameObject Owner, List<GameObject> Victims, Vector3 startPos, Vector3 endPos);

    /// <summary> Called whenever the this weapon is reloaded </summary>
    public event WeaponReloaded EventWeaponReloaded;
    public delegate void WeaponReloaded(GameObject owner, int oldAmmo, int newAmmo);


    protected virtual void Awake() {
        spriteControl = GetComponent<WeaponSpriteController>();
        effectController = GetComponent<FireEffectController>();
    }

    protected virtual void Start() {
        damage = baseDamage;
        if (infiniteReserve)
            reserveSize = int.MaxValue;

        if(owner != null) {
            owner.GetComponent<PlayerWeaponControl>().AddWeapon(this);
            name = owner.name + "'s " + name;
            transform.position = owner.transform.position;
        }
        else {
            Debug.LogWarning("No owner set for " + name);
        }
    }

    public virtual void Update() {
        if(owner == null) {
            return;
        }
        transform.position = owner.transform.position;
    }

    [Server]
    public void SetOwner(Player player) {
        if (player != null)
            owner = player.gameObject;
        SetOwnerRPC(player);
    }
    [ClientRpc]
    private void SetOwnerRPC(Player player) {
        if (player != null)
            owner = player.gameObject;
    }

    // ---- Getters and Setters ----
    public bool IsAutomatic() { return isAutomatic; }
    public int GetReloadAmount() { return reloadAmount; }
    public int GetReserveSize() { return reserveSize; }
    public int GetMagSize() { return magSize; }
    public int GetInMag() { return inMag; }
    public int GetInReserve() {
        if (infiniteReserve)
            return -1;
        return inReserve; 
    }
    public void SetReloadTime(float newTime) { reloadTime = newTime; }
    public float GetDamage() { return damage; }
    public void ResetDamage() { damage = baseDamage;} 
    public void SetDamage(float killDamage) { damage = killDamage; }
    public float GetReloadTime() { return reloadTime; }
    public float GetFireDeley() { return fireDeley; }
    public float GetSwapTime() { return swapTime; }
    public float GetMoveMult() { return movePenalty; }
    public string GetWeaponName() { return weaponName; }
    public bool MagEmpty() { return inMag <= 0;}
    public bool ReserveEmpty() {
        return !infiniteReserve && inReserve <= 0;
    }

    // ---- Play sounds ----
    public void PlayReloadSound() {
        //SoundPlayer.Play(reloadSound, transform.position);
        AudioManager.instance.PlaySound(reloadSound);
    }
    public void PlaySwapSound() {
        //SoundPlayer.Play(swapSound, transform.position);
        AudioManager.instance.PlaySound(swapSound);
    }

    // ---- Internal Weapon Mechanics ----
    /// <summary> Reloads weapon instantly (reloadTime is handled in PlayerWeaponControl script) </summary>
    public virtual void Reload() {
        // Check to make sure total of current mag and reserve is more than a full mag 
        int oldAmmo = inMag;
        if (inReserve + inMag >= magSize) {
            inReserve -= magSize - inMag;
            inMag = magSize;
        }
        else { 
            inMag = inReserve + inMag;  
            inReserve = 0;
        }
        if (EventWeaponReloaded != null) { EventWeaponReloaded.Invoke(owner, oldAmmo, inMag); }
    }
    public void Reload(int magSize) {
        int oldAmmo = inMag;
        if (infiniteReserve) {
            inMag = magSize;
            return;
        }
        int newReloadAmount = (reloadAmount==0)?magSize:reloadAmount;
        int bulletsToReload = ((magSize-inMag) < newReloadAmount) ?(magSize-inMag): newReloadAmount;
        if(inReserve >= bulletsToReload) {
            inReserve -= bulletsToReload;
            inMag += bulletsToReload;
        }
        else {
            inMag = inReserve + inMag;
            inReserve = 0;
        }
        if (infiniteReserve)
            inReserve = -1;

        if(EventWeaponReloaded != null) { EventWeaponReloaded.Invoke(owner, oldAmmo, inMag); }
    }
    /// <summary> adds the given amount of bullets to the reserve</summary>
    public void AddReserveAmmo(int amount) {
        if (infiniteReserve) {
            inReserve = int.MaxValue;
            return;
        }
        inReserve += amount;
    }

    /// <summary> Fires weapon instantly called by PlayerWeaponControl 
    /// (fireDeley is handled in PlayerWeaponControl script) </summary>
    public virtual void Fire(GameObject player, Vector2 direction) {
        inMag--;
        PlayShootSoundForAll();
    }

    // ---- Utility methods not called in this base implementation ----

    // This is NOT called by playerWeaponControl and is just a utility for overriding Fire() in derived Weapon classes
    /// <summary> Fires a shot in the given direction </summary>
    protected void FireShot(GameObject player, Vector2 direction) {
        // Raycast in direction and get all collisions with mask
        string[] mask = { "BulletCollider", "ZombieHitbox", "Door", "Prop"};
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask(mask));
        
        int penetrated = 0; //Used to count how many zombies we collided with and not hit more than weapon's penetration

        Vector2 startPos = spriteControl.BarrelEndPosition();
        Vector2 trailEnd = startPos;
        List<GameObject> victims = new List<GameObject>();
        //Loop through all collisions
        for (int i = 0; i < hitInfos.Length; i++)
        {
            GameObject hitObj = hitInfos[i].transform.gameObject;
            
            if (hitObj.CompareTag("ZombieDamageHitbox")) // We hit a zombie's hitbox
            {
                hitObj = hitObj.GetComponent<DamageHitbox>().owner;
                penetrated++;
                hitObj.GetComponent<ZombieHealth>().DamageCMD(damage, owner);
                victims.Add(hitObj);

                //We have hit our max number of zombies in one shot so we create the trail and break;
                if (penetrated >= penatration){
                    trailEnd = hitInfos[i].point; 
                    break;
                }
            }
            else if (hitObj.CompareTag("BulletCollision") || hitObj.CompareTag("Door"))
            {
                trailEnd = hitInfos[i].point;
                break;
            }
            else if (hitObj.CompareTag("Prop") && hitObj.GetComponent<Prop>().canBeShot) {
                Prop prop = hitObj.GetComponent<Prop>();
                prop.ShootCMD();
                penetrated += prop.hardness;
                if (penetrated >= penatration) {
                    trailEnd = hitInfos[i].point;
                    break;
                }
            }
            else {
                trailEnd = startPos + (direction.normalized * effectController.maxDistance);
            }
        }
        if(trailEnd == startPos)
            trailEnd = startPos + (direction.normalized * effectController.maxDistance);

        if (EventWeaponFired != null) { EventWeaponFired.Invoke(owner, victims, startPos, trailEnd); }
        effectController.CreateTrail(startPos, trailEnd);
        CameraController.instance.Shake(shakeIntensity);
    }
    /// <summary> Fires a shot within the spreadAngle in the given direction </summary>
    protected void FireShot(GameObject player, Vector2 direction, float spreadAngle) {
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x); //Get the angle (in radians) of the direction vector
        float angleDiff = Random.Range(-(spreadAngle / 2), (spreadAngle / 2)); //Get a random float to modify the direction angle
        Vector2 fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
        FireShot(player, fireDir);
    }

    //Sound handling
    public void PlayShootSoundForAll() {
        PlayShootSoundCMD();
    }
    [Command(requiresAuthority = false)]
    private void PlayShootSoundCMD() {
        PlayShootSoundRPC();
    }
    [ClientRpc]
    private void PlayShootSoundRPC() {
        AudioManager.instance.PlaySound(shootSound);
    }

} 
