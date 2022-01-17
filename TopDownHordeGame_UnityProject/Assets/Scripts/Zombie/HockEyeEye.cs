using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockEyeEye : MonoBehaviour
{
    public GameObject eyePrefab;
    private GameObject explosionObj;
    private int balanceDamage;
    private float flySpeed;
    private Vector3 rotationTemp;
    private Vector2 moveDir;
    private float timeActive;
    [SerializeField] private float timeUntilDestroy;

    private GameObject owner;

    public void Init(GameObject newOwner, Vector2 movementDir, int damage, float speed)
    {
        owner = newOwner;
        transform.position = newOwner.transform.position;
        rotationTemp = newOwner.transform.rotation.eulerAngles;
        rotationTemp.z += 90;
        transform.rotation = Quaternion.Euler(rotationTemp);
        moveDir = movementDir;
        balanceDamage = damage;
        flySpeed = speed;
        this.GetComponent<HitBoxController>().EventObjEnter += Impact;
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
    public void Impact(GameObject player)
    {
        Debug.Log("Creating explosion object.");
        timeActive = 0;
        GetComponent<HitBoxController>().EventObjEnter -= Impact;
        Vector3 location = transform.position;
        explosionObj = Instantiate(eyePrefab, location, Quaternion.identity);


        if (player.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().Damage(balanceDamage);
        }
    }

    private void DestroyExplosionObject(GameObject explosionObj)
    {
        Destroy(this);
    }
    [SerializeField] private Rigidbody2D rb;
    private void Move(Vector2 movementDir)
    {
        Vector2 newPos = transform.position;
        newPos += flySpeed * movementDir * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }




}