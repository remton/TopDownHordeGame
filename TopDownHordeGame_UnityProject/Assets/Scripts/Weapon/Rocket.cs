using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Rocket : NetworkBehaviour
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    private float balanceDamage;
    private float throwStrength;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;

    private GameObject owner;

    [ClientRpc]
    public void Init(GameObject newOwner, Vector2 movementDir, float damage, float speed, float knockback)
    {
        if (!isServer) {
            this.enabled = false;
            return;
        }
        owner = newOwner;
        transform.position = newOwner.transform.position;
        moveDir = movementDir;
        balanceDamage = damage;
        throwStrength = knockback;
        flySpeed = speed;
        this.GetComponent<HitBoxController>().EventObjEnter += OnHitSomething;
    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
  
    public void OnHitSomething(GameObject obj) {
        Explode(transform.position);
        GetComponent<HitBoxController>().EventObjEnter -= OnHitSomething;
    }

    //Creates an Explosion Object at location
    [ClientRpc]
    public void Explode(Vector3 location)
    {
        Debug.Log("Creating explosion object.");
        //Stop this from exploding multiple times
        
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);

        List<string> damageTags = new List<string>();
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("ZombieDamageHitbox");
        knockbackTags.Add("Player");

        explosionObj.GetComponent<Explosion>().Init(owner, damageTags, knockbackTags, balanceDamage, throwStrength);
        Destroy(gameObject);
    }

    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir) {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
}