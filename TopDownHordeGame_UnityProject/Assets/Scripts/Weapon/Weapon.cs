using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float reloadTime;  // time in seconds that this weapon takes to reload
    [SerializeField] private float fireDeley;   // time between shots (handled in playerWeaponControl)
    [SerializeField] private float swapTime;   // time to switch to this weapon (handled in playerWeaponControl)
    [SerializeField] private float movePenalty; // subtractive penalty to movespeed when equipped (handled in playerWeaponControl)

    [SerializeField] protected int penatration; // number of zombies able to be hit by one bullet. (Should be at least 1)
    [SerializeField] protected int damage;        // damage per bullet
    private int damageBackup;
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


    /// <summary> Returns true if there is not ammo in the magazine </summary>
    public bool MagEmpty() {
        return inMag <= 0;
    }

    public bool ReserveEmpty() {
        return inReserve <= 0;
    }

    /// <summary> Reloads weapon instantly (reloadTime is handled in PlayerWeaponControl script) </summary>
    public void Reload() {
        if (inReserve + inMag >= magSize) // To Do: Test that this properly reloads the last mag of an RPD. It previously discarded the ammunition in the last magazine. 
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
        if (inReserve + inMag >= magSize) // To Do: Test that this properly reloads the last mag of an RPD. It previously discarded the ammunition in the last magazine. 
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
        if (inReserve > reserveSize)
            inReserve = reserveSize;
    }

    /// <summary> Fires weapon instantly (fireDeley is handled in PlayerWeaponControl script) </summary>
    public virtual void Fire(GameObject player, Vector2 direction) {
        inMag--;
//        Debug.Log(name + ": " + inMag.ToString() + " / " + inReserve.ToString());
    }

    protected void FireShot(GameObject player, Vector2 direction) {
        RaycastHit2D effectRay = Physics2D.Raycast(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask("BulletCollider"));
        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        if (effectRay) {
            Vector2 hitPoint = effectRay.point;
            Vector2 endPos = new Vector3(hitPoint.x, hitPoint.y, 0);
            effectController.CreateTrail(startPos, endPos);
        }
        else {
            effectController.CreateTrailDir(startPos, direction);
        }

        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Zombie"));
        int loopLimit = (penatration < hitInfos.Length) ? penatration : hitInfos.Length; // minimum of penatration or number of hits
        for (int i = 0; i < loopLimit; i++) {
            GameObject zombieHit = hitInfos[i].transform.gameObject;
            if (zombieHit.CompareTag("Zombie")) {
                zombieHit.GetComponent<ZombieHealth>().Damage(damage);
                player.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for shooting someone 
                if (zombieHit.GetComponent<ZombieHealth>().isDead())
                    player.GetComponent<PlayerStats>().AddKill();
            }
        }
    }

}
