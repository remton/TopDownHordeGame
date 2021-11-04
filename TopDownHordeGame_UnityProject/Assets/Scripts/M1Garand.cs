using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1Garand : Weapon
{
    public BulletTrail trail;
    public override void Fire(GameObject player, Vector2 direction) {
        base.Fire(player, direction);
        RaycastHit2D effectRay = Physics2D.Raycast(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Wall"));
        Vector2 startPos = new Vector3(transform.position.x, transform.position.y, 0);
        if (effectRay) {
            Vector2 hitPoint = effectRay.point;
            Vector2 endPos = new Vector3(hitPoint.x, hitPoint.y, 0);
            trail.CreateTrail(startPos, endPos);
        }
        else {
            trail.CreateTrailDir(startPos, direction);
        }


        RaycastHit2D damageRay = Physics2D.Raycast(player.transform.position, direction, Mathf.Infinity, LayerMask.GetMask("Zombie"));
        GameObject zombieHit = damageRay.transform.gameObject;
        if (zombieHit.CompareTag("Zombie")) {
            zombieHit.GetComponent<ZombieHealth>().Damage(damage);
        }
    }
}
