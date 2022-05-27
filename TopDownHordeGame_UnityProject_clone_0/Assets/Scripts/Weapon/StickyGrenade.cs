using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGrenade : MonoBehaviour
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
        gameObject.GetComponent<HitBoxController>().EventObjEnter += Stick;
        timer.CreateTimer(balanceTimer, Explode);
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
  
    //Creates an Explosion Object
    public void Explode(/*GameObject player*/)
    {
        Debug.Log("Creating explosion object.");
        location = transform.position;
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);

        List<string> damageTags = new List<string>();
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("ZombieDamageHitbox");
        knockbackTags.Add("Player");

        explosionObj.GetComponent<Explosion>().Init(owner, damageTags, knockbackTags, balanceDamage, throwStrength);
        Destroy(gameObject); 
    }
    // Sticks to anything the grenade impacts. 
    public void Stick(GameObject objectHit)
    {
        flySpeed = 0;
        stuck = true;
        if (!(objectHit.CompareTag("Player") || objectHit.CompareTag("ZombieDamageHitbox"))) {
            stuckToScenery = true;
        }
        else
        {
            stuckOn = objectHit;
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