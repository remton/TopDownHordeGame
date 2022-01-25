using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Weapon 
{
    [SerializeField] private float fireKnockback = 300F;
    [SerializeField] protected float knockback = 600F; 
    [SerializeField] protected float flySpeed = 30F; 
    public GameObject rocketObjPrefab;
    public GameObject player;
    private Vector3 playerPos;
    protected Vector2 playerLookDir;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        FireRocket(player, direction);
        player.GetComponent<PlayerMovement>().KnockBack(fireKnockback, -direction);
    }

    public void FireRocket(GameObject player, Vector2 direction)
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        playerLookDir = direction;
        GameObject rocket = Instantiate(rocketObjPrefab, playerPos, Quaternion.identity);
        rocket.GetComponent<Rocket>().Init(player, playerLookDir.normalized, damage, flySpeed, knockback);
    }
}