using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePathfind : MonoBehaviour {
    public GameObject target;
    
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    public void SetActive(bool b) {
        agent.updatePosition = b;
        UpdatePath();
    }

    [SerializeField] private float timeBetweenPathUpdates;

    private float timeUntilPathUpdate;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;   
    }

    private void FixedUpdate() {
        // Here we deal with updating our path
        if (timeUntilPathUpdate <= 0) {
            timeUntilPathUpdate = timeBetweenPathUpdates;
            UpdatePath();
        }
        timeUntilPathUpdate -= Time.deltaTime;
    }

    // Faces zombie in the given direction
    private void LookToDir(Vector2 lookDir2D) {
        Vector3 lookDir3D = new Vector3(lookDir2D.x, lookDir2D.y, transform.position.z);
        transform.right = lookDir3D;
    }

    private void UpdatePath() {
        if (target != null) {
            Vector3 pos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            agent.SetDestination(pos);
  //          Debug.Log("path updated");
        }
    }
}
