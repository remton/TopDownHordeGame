using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public GameObject playerToFollow;
    public float moveSpeed;

    // Position to move to
    private Vector2 targetPos;
    private Vector2 moveDir;

    private void Update() {
        MoveTowards(playerToFollow);
        LookAt(moveDir);
    }

    private void LookAt(Vector2 lookDir2D) {
        Vector3 lookDir3D = new Vector3(lookDir2D.x, lookDir2D.y, transform.position.z);
        transform.right = lookDir3D;
    }

    private void MoveTowards(GameObject target) {
        Vector3 targetPos = target.transform.position;
        moveDir = targetPos - transform.position;
        moveDir.Normalize();
        Vector3 moveDir3D = new Vector3(moveDir.x, moveDir.y, transform.position.z);
        transform.position += moveDir3D * moveSpeed * Time.deltaTime;
    }

}
