using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float reloadTime;  // time in seconds that this weapon takes to reload
    [SerializeField] private float fireDeley;   // time between shots (handled in playerWeaponControl)
    [SerializeField] private float swapTime;   // time to switch to this weapon (handled in playerWeaponControl)
    [SerializeField] private float movePenalty; // subtractive penalty to movespeed when equipped (handled in playerWeaponControl)
    [SerializeField] private string weaponName; // Weapon name for display
    [SerializeField] protected int penatration; // number of zombies able to be hit by one bullet. (Should be at least 1)
    [SerializeField] protected int damage;        // damage per bullet
    private int damageBackup;                     // Used to reset damage for the magic that makes players intantly kill zombies
    [SerializeField] protected int magSize;       // size of this weapons magazine
    [SerializeField] protected int reserveSize;   // max ammo that can be held with this weapon
    [SerializeField] protected FireEffectController effectController;
    protected int inMag = 0; // bullets in magazine
    protected int inReserve = 0; // bullets in reserve

    

    public int GetReserveSize() { return reserveSize; }
    public int GetMagSize() { return magSize; }
    public int GetInMag() { return inMag; }
    public int GetInReserve() { return inReserve; }
    public void SetReloadTime(float newTime) { reloadTime = newTime; }
    public int GetDamage() { return damage; }
    public void ResetDamage() { 
        damage = damageBackup; 
    } 
    public void SetKillDamage(int killDamage) {
        damageBackup = damage; 
        damage = killDamage; 
    }
    public float GetReloadTime() { return reloadTime; }
    public float GetFireDeley() { return fireDeley; }
    public float GetSwapTime() { return swapTime; }
    public float GetMoveMult() { return movePenalty; }
    public string GetWeaponName() { return weaponName; }



    /// <summary> Returns true if there is not ammo in the magazine </summary>
    public bool MagEmpty() {
        return inMag <= 0;
    }

    public bool ReserveEmpty() {
        return inReserve <= 0;
    }

    /// <summary> Reloads weapon instantly (reloadTime is handled in PlayerWeaponControl script) </summary>
    public void Reload() {
        if (inReserve + inMag >= magSize) // Check to make sure total of current mag and reserve is more than a full mag 
        {
            inReserve -= magSize - inMag;
            inMag = magSize;
        }
        else { 
            inMag = inReserve + inMag;  
            inReserve = 0;
        }
    }
    public void Reload(int magSize) {
        if (inReserve + inMag >= magSize) // Check to make sure total of current mag and reserve is more than a full mag 
        { 
            inReserve -= magSize - inMag;
            inMag = magSize;
        }
        else {
            inMag = inReserve + inMag;  
            inReserve = 0;
        }
    }

    /// <summary> adds the given amount of bullets to the reserve ammo up to reserveSize </summary>
    public void AddReserveAmmo(int amount) {
        inReserve += amount;
//        if (inReserve > reserveSize)
//            inReserve = reserveSize;
    }

    /// <summary> Fires weapon instantly (fireDeley is handled in PlayerWeaponControl script) </summary>
    public virtual void Fire(GameObject player, Vector2 direction) {
        inMag--;
//        Debug.Log(name + ": " + inMag.ToString() + " / " + inReserve.ToString());
    }

    protected void FireShot(GameObject player, Vector2 direction) {
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask("BulletCollider"));
        int hitZombies = 0;

        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        int loopLimit = hitInfos.Length;
        Debug.Log("loopLimit = " + loopLimit);

        for (int i = 0; i < loopLimit; i++) {
            GameObject zombieHit = hitInfos[i].transform.gameObject;
            if (zombieHit.CompareTag("Zombie"))
            {
                hitZombies++;
                Debug.Log("   Zombie " + i + " is: " + zombieHit);
                zombieHit.GetComponent<ZombieHealth>().Damage(damage);
                player.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for shooting someone 
                if (zombieHit.GetComponent<ZombieHealth>().isDead())
                {
                    player.GetComponent<PlayerStats>().AddKill();
                    Debug.Log("      Zombie" + i + "died.");
                }

                if (hitZombies >= penatration)
                {
                    Debug.Log("Drawing to " + zombieHit + ". i = " + i + "/" + loopLimit);
                    Vector2 hitPoint = zombieHit.transform.position;
                    Vector2 endPos = new Vector3(hitPoint.x, hitPoint.y, 0);
                    effectController.CreateTrail(startPos, endPos);
                    break;
                }
                
            }
            if (zombieHit.CompareTag("Wall"))
            {
                Debug.Log("Drawing to the wall");
                Vector2 hitPoint = hitInfos[i].point;
                Vector2 endPos = new Vector3(hitPoint.x, hitPoint.y, 0);
                effectController.CreateTrail(startPos, endPos);
                break;
            }
        }
    }
} 
