using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RPG : Weapon 
{
    [SerializeField] private float fireKnockback = 300F;
    [SerializeField] protected float knockback = 600F; 
    [SerializeField] protected float flySpeed = 30F; 
    public GameObject rocketObjPrefab;
    public GameObject player;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        FireRocket(player, direction);
        player.GetComponent<PlayerMovement>().KnockBack(fireKnockback, -direction);
    }

    [Command(requiresAuthority = false)]
    public void FireRocket(GameObject player, Vector2 direction)
    {
        GameObject rocket = Instantiate(rocketObjPrefab, spriteControl.BarrelEndPosition(), Quaternion.identity);
        rocket.transform.right = new Vector3(direction.x, direction.y, transform.position.z);
        NetworkServer.Spawn(rocket);
        rocket.GetComponent<Rocket>().Init(player, direction.normalized, damage, flySpeed, knockback);
    }
}