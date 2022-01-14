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
    private Vector2 moveDir;
    private float timeActive;
    [SerializeField] private float timeUntilDestroy;

    private GameObject owner;

    public void Init(GameObject newOwner, Vector2 movementDir, int damage, float radius, float speed)
    {
        owner = newOwner; 
        transform.position = newOwner.transform.position;
        rotationTemp = newOwner.transform.rotation.eulerAngles;
        rotationTemp.z += 90;
        transform.rotation = Quaternion.Euler(rotationTemp);
        moveDir = movementDir;
        balanceDamage = damage;
        balanceRadius = radius;
        flySpeed = speed;
        this.GetComponent<HitBoxController>().EventObjEnter += Explode;
    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
    private void Update()
    {
        if (explosionObj)
        {
            if (timeActive >= timeUntilDestroy)
            {
                DestroyExplosionObject(explosionObj);
                timeActive = 0;
            }
            else
            {
                timeActive += Time.deltaTime;
            }
        }
    }
    public void Explode(GameObject player)
    {
        Debug.Log("Creating explosion object.");
        timeActive = 0;
        GetComponent<HitBoxController>().EventObjEnter -= Explode;
        Vector3 location = transform.position;
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);
        
        explosionObj.transform.position = location;
        Vector3 balanceScale;
        balanceScale.x = balanceRadius;
        balanceScale.y = balanceRadius;
        balanceScale.z = balanceRadius;

        HideRocketObject();

        Debug.Log("Resizing explosion object.");
        explosionObj.transform.localScale = balanceScale;
        explosionObj.GetComponent<HitBoxController>().EventObjEnter += DamageZombies;
    }
    private void HideRocketObject()
    {
        Vector3 hide; 
//        this.GetComponent<HitBoxController>().EventObjEnter -= DamageZombies;
        flySpeed = 0;
        hide.x = transform.position.x * 10;
        hide.y = transform.position.y * 10;
        hide.z = transform.position.z * 10;
        transform.position = hide;
        Debug.Log("Rocket should be hidden.");
    }
    private void DestroyExplosionObject(GameObject explosionObj)
    {
        this.GetComponent<HitBoxController>().EventObjEnter -= DamageZombies;
        Destroy(explosionObj);
        Destroy(this);
    }
    public void DamageZombies(GameObject zombie)
    {
        zombie.GetComponent<ZombieHealth>().Damage(balanceDamage);
        owner.GetComponent<PlayerStats>().AddMoney(1); // Give the player money for the explosion hitting someone 
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