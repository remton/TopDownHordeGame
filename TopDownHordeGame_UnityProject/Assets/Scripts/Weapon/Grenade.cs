using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosionPrefab;
    private GameObject explosionObj;
    private int balanceDamage;
    private float throwStrength;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private float balanceTimer;
    private Timer timer;

    private GameObject owner;

    public void Init(GameObject newOwner, Vector2 movementDir, int damage, float radius, float speed, float knockback, float explodeTime)
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
        timer = GetComponent<Timer>();
        gameObject.GetComponent<HitBoxController>().EventObjEnter += Ricochet;
        timer.CreateTimer(balanceTimer, Explode);
    }

    private void FixedUpdate()
    {
        Move(moveDir);
    }
  
    //Creates an Explosion Object
    public void Explode(/*GameObject player*/)
    {
        Debug.Log("Creating explosion object.");
        //Stop this from exploding multiple times
        //GetComponent<HitBoxController>().EventObjEnter -= Explode;
        Vector3 location = transform.position;
        explosionObj = Instantiate(explosionPrefab, location, Quaternion.identity);

        List<string> damageTags = new List<string>();
        damageTags.Add("Zombie");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("Zombie");
        knockbackTags.Add("Player");

        explosionObj.GetComponent<Explosion>().Init(owner, damageTags, knockbackTags, balanceDamage, throwStrength);
        Destroy(gameObject); 
    }

    private void Ricochet(GameObject objectHit)
    {
        // Reflecting Bullets
        Quaternion normal = objectHit.transform.rotation;
        Vector2 normalVector = normal.eulerAngles;
        Vector2 v = Vector2.Reflect(transform.up, normalVector);
        float rot = Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0, 0, rot);
        moveDir = transform.eulerAngles;
    }

    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir) {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
   
}