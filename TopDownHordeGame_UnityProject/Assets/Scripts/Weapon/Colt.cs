using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remi named this
public class Colt : Weapon{
    private GameObject player;

    private void FireRicochet(Vector2 startPos, Vector2 direction, int bounceNum) {
        // Raycast in direction and get first collsion with mask
        string[] mask = { "BulletCollider", "Zombie", "Door" };
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask(mask));
        // We hit nothing
        if (hitInfos.Length == 0) {
            Vector2 trailEnd = startPos + (direction.normalized * effectController.maxDistance);
            effectController.CreateTrail(startPos, trailEnd);
            return;
        }
        GameObject hitObj = hitInfos[0].transform.gameObject;
        Vector2 hitPoint = hitInfos[0].point;
        int finalhitIndex = 0;
        if (hitObj.CompareTag("Zombie")) {
            //Loop until we dont hit a zombie -_-
            for (int i = 0; i < hitInfos.Length; i++) {
                finalhitIndex = i;
                hitObj = hitInfos[i].transform.gameObject;
                hitPoint = hitInfos[i].point;
                if (hitObj.tag != "Zombie" || Random.Range(0,2)==0)
                    break;
                if (i>0)//Double damage for every zombie penetrated!
                    hitObj.GetComponent<ZombieHealth>().Damage(damage*2*i);
                hitObj.GetComponent<ZombieHealth>().Damage(damage);
                player.GetComponent<PlayerStats>().PayForHit();
                if (hitObj.GetComponent<ZombieHealth>().isDead()) {
                    player.GetComponent<PlayerStats>().AddKill();
                }
            }
        }
        effectController.CreateTrail(startPos, hitPoint);
        Vector2 reflection = Vector2.Reflect(hitPoint - startPos, hitInfos[finalhitIndex].normal);
        bounceNum++;
        if (bounceNum >= penatration)
            return;
        else
            FireRicochet(hitPoint, reflection.normalized, bounceNum);
    }

    public override void Fire(GameObject player, Vector2 direction) {
        this.player = player;
        base.Fire(player, direction); 
        Vector2 startPos = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        FireRicochet(startPos, direction, 0);
    }
}
