using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remi named this
public class Colt : Weapon{
    private GameObject player;
    [SerializeField] private float spreadAngleOnRicochet;
    const float bounceShiftForward = 0.1f;

    private List<GameObject> victims = new List<GameObject>();
    
    private void FireRicochet(Vector2 startPos, Vector2 direction, int bounceNum) {
        // Raycast in direction and get first collsion with mask
        string[] mask = { "BulletCollider", "PlayerHitbox", "ZombieHitbox", "Door", "Prop" }; ;
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(startPos, direction, Mathf.Infinity, LayerMask.GetMask(mask));
        // We hit nothing
        if (hitInfos.Length == 0) {
            Vector2 trailEnd = startPos + (direction.normalized * effectController.maxDistance);
            effectController.CreateTrail(startPos, trailEnd);
            return;
        }

        GameObject hitObj = hitInfos[0].transform.gameObject;
        Vector2 hitPoint = hitInfos[0].point;
        int finalhitIndex = 0;
        float damageMult = 1f;
        for (int i = 0; i < hitInfos.Length; i++) {
            finalhitIndex = i;
            hitObj = hitInfos[i].transform.gameObject;
            hitPoint = hitInfos[i].point;
            if (hitObj.CompareTag("BulletCollision") || hitObj.CompareTag("Door"))
                break;

            if (hitObj.CompareTag("PlayerDamageHitbox")) {
                hitObj = hitObj.GetComponent<DamageHitbox>().owner;
                if (hitObj != owner && hitObj.GetComponent<PlayerHealth>().HasFriendlyFire()) {
                    hitObj.GetComponent<PlayerHealth>().DamageCMD(damage * damageMult);
                    damageMult++;
                    victims.Add(hitObj);
                }
                else
                    continue;
            }
            else if (hitObj.CompareTag("ZombieDamageHitbox")) {
                hitObj = hitObj.GetComponent<DamageHitbox>().owner;
                hitObj.GetComponent<ZombieHealth>().DamageCMD(damage * damageMult, owner);
                damageMult++;
                victims.Add(hitObj);
            }
            else if (hitObj.CompareTag("Prop") && hitObj.GetComponent<Prop>().canBeShot) {
                Prop prop = hitObj.GetComponent<Prop>();
                prop.ShootCMD();
                break;
            }

            if (Random.Range(0, 2) == 0)
                break;
        }
        effectController.CreateTrail(startPos, hitPoint);
        Vector2 reflection = Vector2.Reflect(hitPoint - startPos, hitInfos[finalhitIndex].normal);
        float spread = spreadAngleOnRicochet * bounceNum;
        float baseAngle = Mathf.Atan2(reflection.y, reflection.x); //Get the angle (in radians) of the direction vector
        float angleDiff = Random.Range(-(spread / 2), (spread / 2)); //Get a random float to modify the direction angle
        reflection = new Vector2(Mathf.Cos(baseAngle + angleDiff * Mathf.Deg2Rad), Mathf.Sin(baseAngle + angleDiff * Mathf.Deg2Rad));
        bounceNum++;
        //BASE CASE
        if (bounceNum >= penatration) {
            CallFireShotEvent(victims, startPos, hitPoint);
            CameraController.instance.Shake(shakeIntensity);
            return;
        }
        else {/*Unless*/}
        //RECURSION
        FireRicochet(hitPoint+(reflection.normalized*bounceShiftForward), reflection.normalized, bounceNum);
    }

    public override void Fire(GameObject player, Vector2 direction) {
        victims.Clear();
        this.player = player;
        base.Fire(player, direction);
        CameraController.instance.Shake(shakeIntensity / 35);
        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        FireRicochet(startPos, direction, 0);
    }
}
