using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    private int balanceDamage;
    private float balanceRadius;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Quaternion roatationQuat;
    private Vector2 moveDir; 

    private GameObject owner;

    public void Init(GameObject newOwner, Vector2 movementDir, int damage, float radius, float speed)
    {
        owner = newOwner; 
        transform.position = newOwner.transform.position;
        rotationTemp = newOwner.transform.rotation.eulerAngles;
        rotationTemp.z += 90;
        transform.rotation = Quaternion.Euler(rotationTemp);
//        roatationQuat = 
        moveDir = movementDir;//newOwner.GetComponent<PlayerMovement>().GetCurrentLookDir();
        balanceDamage = damage;
        balanceRadius = radius;
        flySpeed = speed;
    }


    private void Awake()
    {

    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
        public void Explode()
    {
        Debug.Log("Rocket should have exploded. ");
        explosionObj = Instantiate(explosionPrefab, transform);
        explosionObj.transform.position = transform.position;
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
        owner.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for explosion hitting someone 
        if (zombie.GetComponent<ZombieHealth>().isDead())
            owner.GetComponent<PlayerStats>().AddKill();
    }
    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }




}