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
    private ZombieWindowAttack windowAttack;

    // Position to move to
    private Vector2 targetPos;
    private Vector2 moveDir;

    public float playerDistForLunge;
    private bool isLunging;

    [SerializeField] private float lungeCooldown;

    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;
    private bool inside = false; // ERIC TEST
    [SerializeField] private GameObject window;
    [SerializeField] private float windowAttackDistance; 
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        zombieLunge = GetComponent<ZombieLunge>();
        windowAttack = GetComponent<ZombieWindowAttack>();
    }

    private void Update() {
        LookToDir(moveDir);
        if (!inside && Vector2.Distance(window.transform.position, transform.position) <= windowAttackDistance )
        {
            if (!window.GetComponent<Window>().GetIsOpen())
            {
                windowAttack.WindowAttack(window.GetComponent<Window>()); 
            }
            else
            {
                window.GetComponent<Window>().MoveToInside(gameObject);
            }
        }
         else if (!lungeOnCooldown && Vector2.Distance(playerToFollow.transform.position, transform.position) <= playerDistForLunge) {
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
        if (!isLunging)
            if (inside)
            {
                MoveTowards(playerToFollow.transform.position);
            }
            else
            {
                MoveTowards(window.transform.position);
            }
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

    public void SetWindow(GameObject newWindow) {

    }

}
