using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SGL : Weapon 
{
    [SerializeField] protected float balanceRadius = 5F; // Radius for grenade 
    protected float knockback = 600F; 
    protected float flySpeed = 30F; 
    public GameObject grenadeObjPrefab;
    public GameObject player;
    private Vector3 playerPos;
    protected Vector2 playerLookDir;
    protected float explodeTime = 1.5f;

    public override void Fire(GameObject player, Vector2 direction)
    {
        base.Fire(player, direction);
        FireGrenade(player, direction);
    }

    [Command(requiresAuthority = false)]
    public void FireGrenade(GameObject player, Vector2 direction)
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        playerLookDir = direction;
        GameObject grenade = Instantiate(grenadeObjPrefab, playerPos, Quaternion.identity);
        NetworkServer.Spawn(grenade);
        grenade.GetComponent<StickyGrenade>().Init(player, playerLookDir.normalized, damage, balanceRadius, flySpeed, knockback, explodeTime);
    }
}