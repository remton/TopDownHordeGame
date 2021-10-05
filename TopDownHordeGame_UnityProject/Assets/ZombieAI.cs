using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public GameObject playerToFollow;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;

    private Rigidbody2D rb;
    private ZombieLunge zombieLunge;

    // Position to move to
    private Vector2 targetPos;
    private Vector2 moveDir;

    public float playerDistForLunge;
    private bool isLunging;

    [SerializeField] private float lungeCooldown;

    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        zombieLunge = GetComponent<ZombieLunge>();
    }

    private void Update() {
        LookToDir(moveDir);
        if(!lungeOnCooldown && Vector2.Distance(playerToFollow.transform.position, transform.position) <= playerDistForLunge) {
            zombieLunge.Lunge(moveDir);
            isLunging = true;
        }

        //Lunge cooldown management
        if (lungeOnCooldown) {
            timeUntilLungeCooldown -= Time.deltaTime;
            if(timeUntilLungeCooldown <= 0)
                lungeOnCooldown = false;
        }
    }

    //called after every frame
    private void FixedUpdate() {
        if(!isLunging)
            MoveTowards(playerToFollow.transform.position);
    }

    // Faces zombie in the given direction
    private void LookToDir(Vector2 lookDir2D) {
        Vector3 lookDir3D = new Vector3(lookDir2D.x, lookDir2D.y, transform.position.z);
        transform.right = lookDir3D;
    }

    //moves toward the given position
    //should be called in fixedUpdate
    private void MoveTowards(Vector3 targetPos) {
        moveDir = targetPos - transform.position;
        moveDir.Normalize();
        Vector2 newPos = transform.position;
        newPos += moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    public void OnLungeEnd() {
        isLunging = false;
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
    }

}
