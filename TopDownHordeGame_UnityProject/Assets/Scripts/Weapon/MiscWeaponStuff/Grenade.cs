using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grenade : NetworkBehaviour
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    private float balanceDamage;
    private float throwStrength;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private float balanceTimer;
    [SerializeField] private float dragCoefficient;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Timer timer;
    [SerializeField] private Rigidbody2D rb;
    private float bounceCoefficient;

    private GameObject owner;

    [ClientRpc]
    public void Init(GameObject newOwner, Vector2 movementDir, float damage, float radius, float speed, float knockback, float explodeTime)
    {
        //Debug.Log("Grenade Init ran");
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

        if (!isServer) {
            this.enabled = false;
            return;
        }

        timer = GetComponent<Timer>();
        timer.CreateTimer(balanceTimer, Explode);
        bounceCoefficient = 0.8f;
        //Debug.Log("Grenade Init ended");
    }

    private void FixedUpdate()
    {
        int count = rb.Cast(moveDir, castCollisions, flySpeed * Time.fixedDeltaTime); // casts a ray based on where the grenade is about to move, saves each collision in castCollisions, and saves number of collisions in count

        if (count != 0)
        {
            for (int i = 0; i < count; i++) // not sure how necessary this is
            {
                if (castCollisions[i].collider.CompareTag("BulletCollision") || castCollisions[i].collider.CompareTag("ZombieDamageHitbox")) // ricochets if collider has the right tag
                {
                    Ricochet(castCollisions[i]);
                    Move(moveDir);
                    return;
                }
            }
        }
        Move(moveDir);
    }
  
    //Creates an Explosion Object
    [ClientRpc]
    public void Explode(/*GameObject player*/)
    {
        //Debug.Log("Creating explosion object.");
        //Stop this from exploding multiple times
        //GetComponent<HitBoxController>().EventObjEnter -= Explode;
        Vector3 location = transform.position;
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);

        List<string> damageTags = new List<string>();
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("ZombieDamageHitbox");
        knockbackTags.Add("Player");

        explosionObj.GetComponent<Explosion>().Init(owner, damageTags, knockbackTags, balanceDamage, throwStrength);
        Destroy(gameObject); 
    }

    private void Ricochet(RaycastHit2D hitInfo) // bounces grenade off object, and decreases its speed
    {
        // Reflecting Bullets
        //Debug.Log("Grenade Ricochet ran");
        
        Vector2 reflectedVector = Vector2.Reflect(moveDir, hitInfo.normal);
        moveDir = reflectedVector;
        flySpeed *= bounceCoefficient;

        //Transform hitObject = hitInfo.transform;
        //Debug.Log("object hit: " + hitObject.name);
    }

    private void Move(Vector2 movementDir) {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private void Update()
    {
        flySpeed *= dragCoefficient;
    }

}