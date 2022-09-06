using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
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
    private int bounceNum;
    private Timer timer;

    private GameObject owner;

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
        timer = GetComponent<Timer>();
        gameObject.GetComponent<HitBoxController>().EventObjEnter += Ricochet;
        timer.CreateTimer(balanceTimer, Explode);
        bounceNum = 0;
        //Debug.Log("Grenade Init ended");
    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
  
    //Creates an Explosion Object
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

    /* This function seems to mostly work when the (x, y, z) scale on the Grenade prefab is set to (1, 1, 1)
     * 
     * Known issues:
     * Can phase through corners (after grenade hits vertical wall, it can sometimes pass through horizontal wall)
     * Can get stuck in walls (stops happening when hitbox is really big)
     */
    private void Ricochet(GameObject objectHit)
    {
        // Reflecting Bullets
        //Debug.Log("Grenade Ricochet ran");
        string[] mask = { "BulletCollider", "ZombieHitbox", "Door" };
        Vector2 normalVector;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDir, 3, LayerMask.GetMask(mask)); // Raycasting is okay here, since this function is only called during collision
        Vector2 hitPoint = hitInfo.point;
        //Debug.Log("starting move direction: " + moveDir);
        //Debug.Log("normal vector: " + hitInfo.normal);
        Vector2 position2d = transform.position;
        Vector2 reflectedVector = Vector2.Reflect((hitPoint - position2d).normalized, hitInfo.normal);
        moveDir = reflectedVector;
        //Debug.Log("final move direction: " + reflectedVector);
        flySpeed *= 0.8f;

        // This next part is intended to stop the grenade from phasing through corners sometimes

        RaycastHit2D nextHitInfo = Physics2D.Raycast(transform.position, moveDir, 2, LayerMask.GetMask(mask));
        
        if (nextHitInfo.collider == null)
        {
            Debug.Log("nextHitInfo is null");
        } else if (bounceNum <= 1)
        {
            GameObject nextObjectHit = nextHitInfo.transform.gameObject;
            bounceNum++;
            Ricochet(nextObjectHit);
        }
        bounceNum = 0;

        /* What was in this function before I started working on it
        Quaternion normal = objectHit.transform.rotation;
        Vector2 normalVector = normal.eulerAngles;
        Vector2 v = Vector2.Reflect(transform.up, normalVector);
        Debug.Log("moveDir = " + moveDir);
        Debug.Log("normalVector = " + normalVector);
        Debug.Log("reflected vector = " + v);
        float rot = Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, rot);
        moveDir = transform.eulerAngles;
        Debug.Log("moveDir = " + moveDir);
        */
    }

    [SerializeField] private Rigidbody2D rb;
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