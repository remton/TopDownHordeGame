using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StickyGrenade : NetworkBehaviour
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    private float balanceDamage;
    private float throwStrength;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private float balanceTimer;
    private Timer timer;
    private bool stuck;
    private bool stuckToScenery;
    private GameObject stuckOn;
    Vector3 location;

    private GameObject owner;

    [ClientRpc]
    public void Init(GameObject newOwner, Vector2 movementDir, float damage, float radius, float speed, float knockback, float explodeTime)
    {
        owner = newOwner; 
        transform.position = newOwner.transform.position;
        rotationTemp = newOwner.transform.rotation.eulerAngles;
        rotationTemp.z += 90;
        transform.rotation = Quaternion.Euler(rotationTemp);
        moveDir = movementDir;
        balanceDamage = damage;
        throwStrength = knockback;
        flySpeed = speed;
        balanceTimer = explodeTime;
        stuck = false;
        stuckToScenery = false;
        timer = GetComponent<Timer>();

        if (!isServer) {
            this.enabled = false;
            return;
        }

        gameObject.GetComponent<HitBoxController>().EventObjEnter += Stick;
        timer.CreateTimer(balanceTimer, OnTimerEnd);
    }

    private void FixedUpdate()
    {
        Move(moveDir);
        if (stuck && !stuckToScenery && stuckOn != null)
        {
            location = stuckOn.transform.position;
            transform.position = location;
        }
    }
  
    [Server]
    private void OnTimerEnd() {
        Explode(transform.position);
    }

    //Creates an Explosion Object
    [ClientRpc]
    public void Explode(Vector3 location)
    {
        Debug.Log("Creating explosion object.");
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);

        List<string> damageTags = new List<string>();
        damageTags.Add("ZombieDamageHitbox");
        damageTags.Add("PlayerDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("ZombieDamageHitbox");
        knockbackTags.Add("Player");

        explosionObj.GetComponent<Explosion>().Init(owner, damageTags, knockbackTags, balanceDamage, throwStrength);
        Destroy(gameObject); 
    }
    // Sticks to anything the grenade impacts. 
    public void Stick(GameObject objectHit)
    {
        GameObject ownerObj = null;
        if (objectHit.HasComponent<DamageHitbox>())
            ownerObj = objectHit.GetComponent<DamageHitbox>().owner;
        if (objectHit == owner || ownerObj == owner) {
            return;
        }

        flySpeed = 0;
        stuck = true;

        if (objectHit.CompareTag("PlayerDamageHitbox")) {
            stuckOn = objectHit;
        }
        else if (objectHit.CompareTag("ZombieDamageHitbox")) {
            stuckOn = objectHit;
        }
        else {
            stuckToScenery = true;
        }
        gameObject.GetComponent<HitBoxController>().EventObjEnter -= Stick;
    }

    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir) {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
   
}