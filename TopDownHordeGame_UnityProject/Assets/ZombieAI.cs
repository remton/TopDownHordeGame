using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public GameObject playerToFollow;
    public float moveSpeed;

    [SerializeField] Rigidbody2D rb;

    // Position to move to
    private Vector2 targetPos;
    private Vector2 moveDir;

    private void Update() {
        LookToDir(moveDir);
    }

    //called after every frame
    private void FixedUpdate() {
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

}
