using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Weapon 
{
    protected int balanceDamage = 4; // Damage for reload 
    protected float balanceRadius = 1.8F; // Radius for reload 
    protected float flySpeed = 3F; 
    public GameObject rocketObj; 
    public GameObject rocketObjPrefab;
    public GameObject player;
    private Vector3 playerPos;
    protected Vector2 playerLookDir;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        FireRocket(player, direction);
    }

    public void FireRocket(GameObject player, Vector2 direction)
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        Instantiate(rocketObjPrefab, playerPos, Quaternion.identity);
        playerLookDir = direction;
    } 
    public float GetFlySpeed()
    {
        return flySpeed;
    }
}