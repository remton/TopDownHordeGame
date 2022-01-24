using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected bool isAutomatic;    // True if this weapon is automatic
    [SerializeField] private float reloadTime;      // time in seconds that this weapon takes to reload
    [SerializeField] protected float fireDeley;     // time between shots (handled in playerWeaponControl)
    [SerializeField] private float swapTime;        // time to switch to this weapon (handled in playerWeaponControl)
    [SerializeField] private float movePenalty;     // subtractive penalty to movespeed when equipped (handled in playerWeaponControl)
    [SerializeField] private string weaponName;     // Weapon name for display
    [SerializeField] protected int penatration;     // number of zombies able to be hit by one bullet. (Should be at least 1)
    [SerializeField] private int baseDamage;        // Used to reset damage for the magic that makes players intantly kill zombies
    [SerializeField] protected int reloadAmount;    // amount of bullets to put in mag on reload (used in weapons like the RemTon870)
    [SerializeField] protected int magSize;         // size of this weapons magazine
    [SerializeField] protected int reserveSize;     // max ammo that can be held with this weapon
    [SerializeField] public AudioClip fireSound;    //sound of gunshot for this weapon
    [SerializeField] public AudioClip reloadSound;  //sound played on reload
    [SerializeField] public AudioClip swapSound;    //sound played when swapping to this weapon
    [SerializeField] protected FireEffectController effectController;
    private AudioSource audioSource; //What will play our sounds

    protected int damage;       // damage per bullet
    protected int inMag = 0;    // bullets in magazine
    protected int inReserve = 0;// bullets in reserve

    
    private void Start() {
        damage = baseDamage;
        if (gameObject.HasComponent<AudioSource>()) {
            audioSource = GetComponent<AudioSource>();
        }
        else {
            Debug.LogError("Weapon: " + name + " is missing its audioSource component!");
        }
    }
    // ---- Getters and Setters ----
    public bool IsAutomatic() { return isAutomatic; }
    public int GetReloadAmount() { return reloadAmount; }
    public int GetReserveSize() { return reserveSize; }
    public int GetMagSize() { return magSize; }
    public int GetInMag() { return inMag; }
    public int GetInReserve() { return inReserve; }
    public void SetReloadTime(float newTime) { reloadTime = newTime; }
    public int GetDamage() { return damage; }
    public void ResetDamage() { damage = baseDamage;} 
    public void SetKillDamage(int killDamage) { damage = killDamage; }
    public float GetReloadTime() { return reloadTime; }
    public float GetFireDeley() { return fireDeley; }
    public float GetSwapTime() { return swapTime; }
    public float GetMoveMult() { return movePenalty; }
    public string GetWeaponName() { return weaponName; }
    public bool MagEmpty() { return inMag <= 0;}
    public bool ReserveEmpty() {return inReserve <= 0;}

    // ---- Play sounds ----
    //public void PlayGunshotSound() {
    //    audioSource.clip = fireSound;
    //    audioSource.Play();
    //}
    public void PlayReloadSound() {
        audioSource.clip = reloadSound;
        audioSource.Play();
    }
    public void PlaySwapSound() {
        audioSource.clip = swapSound;
        audioSource.Play();
    }

    // ---- Internal Weapon Mechanics ----
    /// <summary> Reloads weapon instantly (reloadTime is handled in PlayerWeaponControl script) </summary>
    public void Reload() {
        // Check to make sure total of current mag and reserve is more than a full mag 
        if (inReserve + inMag >= magSize) {
            inReserve -= magSize - inMag;
            inMag = magSize;
        }
        else { 
            inMag = inReserve + inMag;  
            inReserve = 0;
        }
    }
    public void Reload(int magSize) {
        //PlaySound
        //PlayReloadSound();

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
    }
    /// <summary> adds the given amount of bullets to the reserve ammo up to reserveSize </summary>
    public void AddReserveAmmo(int amount) {
        inReserve += amount;
    }

    /// <summary> Fires weapon instantly called by PlayerWeaponControl 
    /// (fireDeley is handled in PlayerWeaponControl script) </summary>
    public virtual void Fire(GameObject player, Vector2 direction) {
        inMag--;
    }

    // ---- Utility methods not called in this base implementation ----

    // This is NOT called by playerWeaponControl and is just a utility for overriding Fire() in derived Weapon classes
    /// <summary> Fires a shot in the given direction </summary>
    protected void FireShot(GameObject player, Vector2 direction) {
        //PlayGunshotSound();

        // Raycast in direction and get all collisions with mask
        string[] mask = { "BulletCollider", "Zombie", "Door"};
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask(mask));
        
        int hitZombies = 0; //Used to count how many zombies we collided with and not hit more than weapon's penetration
        
        //Loop through all collisions
        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        for (int i = 0; i < hitInfos.Length; i++)
        {
            GameObject hitObj = hitInfos[i].transform.gameObject;
            if (hitObj.CompareTag("Zombie")) // We hit a zombie
            {
                hitZombies++;
                hitObj.GetComponent<ZombieHealth>().Damage(damage);
                player.GetComponent<PlayerStats>().PayForHit();
                if (hitObj.GetComponent<ZombieHealth>().isDead()){
                    player.GetComponent<PlayerStats>().AddKill();
                }

                //We have hit our max number of zombies in one shot so we create the trail and break;
                if (hitZombies == penatration){
                    Vector2 hitPoint = hitInfos[i].point;
                    effectController.CreateTrail(startPos, hitPoint);
                    break;
                }
            }
            else if (hitObj.CompareTag("BulletCollision") || hitObj.CompareTag("Door"))
            {
                Vector2 hitPoint = hitInfos[i].point;
                effectController.CreateTrail(startPos, hitPoint);
                break;
            }
            else {
                Vector2 trailEnd = startPos + (direction.normalized * effectController.maxDistance);
                effectController.CreateTrail(startPos, trailEnd);
            }
        }
    }
    /// <summary> Fires a shot within the spreadAngle in the given direction </summary>
    protected void FireShot(GameObject player, Vector2 direction, float spreadAngle) {
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x); //Get the angle (in radians) of the direction vector
        float angleDiff = Random.Range(-(spreadAngle / 2), (spreadAngle / 2)); //Get a random float to modify the direction angle
        Vector2 fireDir = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
        FireShot(player, fireDir);
    }
} 
