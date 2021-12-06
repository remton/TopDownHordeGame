using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePathfind : MonoBehaviour {
    public GameObject target;
    
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    public void SetActive(bool b) {
        agent.enabled = b;
        UpdatePath();
    }
    public void Activate(float waitTime) {
        timeUntilSetActive = waitTime;
        waitingToActivate = true;
    }
    private float timeUntilSetActive;
    private bool waitingToActivate = false;

    [SerializeField] private float timeBetweenPathUpdates;

    private float timeUntilPathUpdate;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false; // navmeshagent starts disabled to fix spawning teleportation issues
    }

    private void Update() {
        // Handle Activate off delay
        if (waitingToActivate) {
            timeUntilSetActive -= Time.deltaTime;
            if (timeUntilSetActive <= 0) {
                waitingToActivate = false;
                SetActive(true);
            }
        }
    }

    private void FixedUpdate() {
        if (!agent.enabled)
            return;

        // Here we deal with updating our path
        if (timeUntilPathUpdate <= 0) {
            timeUntilPathUpdate = timeBetweenPathUpdates;
            UpdatePath();
        }
        timeUntilPathUpdate -= Time.fixedDeltaTime;
    }

    private void UpdatePath() {
        if (target != null && agent.enabled) {
            Vector3 pos = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            agent.SetDestination(pos);
  //          Debug.Log("path updated");
        }
    }
}
