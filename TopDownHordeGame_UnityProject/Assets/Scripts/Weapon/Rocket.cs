using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : RPG
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;

    private Vector2 moveDir; 

    private void Awake()
    {
        transform.position = player.transform.position;
        //        moveDir = player.GetComponent<PlayerMovement>.GetCurrentLookDirection;
    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
        public void ExplosionDamage(GameObject player)
    {
        this.player = player;
        Debug.Log("Rocket should have exploded. ");
        explosionObj = Instantiate(explosionPrefab, transform);
        explosionObj.transform.position = player.transform.position;
        Vector3 balanceScale;
        balanceScale.x = balanceRadius;
        balanceScale.y = balanceRadius;
        balanceScale.z = balanceRadius;

        explosionObj.transform.localScale = balanceScale;
        explosionObj.GetComponent<HitBoxController>().EventObjEnter += DamageZombies;

    }
    private void DestroyRocketObject(GameObject explosionObj)
    {
        this.GetComponent<HitBoxController>().EventObjEnter -= DamageZombies;
        Destroy(this);
    }

    public void DamageZombies(GameObject zombie)
    {
        zombie.GetComponent<ZombieHealth>().Damage(balanceDamage);
        player.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for explosion hitting someone 
        if (zombie.GetComponent<ZombieHealth>().isDead())
            player.GetComponent<PlayerStats>().AddKill();
    }
    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;
 //       newPos += GetComponent<RPG>.GetFlySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }




}