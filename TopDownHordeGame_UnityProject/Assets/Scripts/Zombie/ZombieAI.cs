using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public GameObject target;

    private ZombieLunge zombieLunge;
    private ZombiePathfind zombiePath;
    private ZombieHealth zombieHealth;

    // Position to move to
    private Vector2 targetPos;

    public float playerDistForLunge;
    private bool isLunging;

    [SerializeField] private float lungeCooldown;

    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;

    public void SetValues(int newHealth, float newSpeed, int newDamage) {
        zombieHealth.SetMaxHealth(newHealth);
        zombiePath.target = target;
    }

    private void FindTarget() {
        target = GameObject.FindGameObjectWithTag("Player");
        zombiePath.target = target;
    }

    private void Awake() {
        zombieLunge = GetComponent<ZombieLunge>();
        zombiePath = GetComponent<ZombiePathfind>();
        zombieHealth = GetComponent<ZombieHealth>();
        FindTarget();
    }

    private void Start() {
        zombiePath.Activate(2*Time.deltaTime);
    }

    private void Update() {
        //zombie lunges
        if (target!=null && !lungeOnCooldown && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            zombiePath.SetActive(false);
            Vector2 dir = target.transform.position - transform.position;
            zombieLunge.Lunge(dir);
            isLunging = true;
        }

        //Lunge cooldown management
        if (lungeOnCooldown) {
            timeUntilLungeCooldown -= Time.deltaTime;
            if(timeUntilLungeCooldown <= 0)
                lungeOnCooldown = false;
        }
    }

    public void OnLungeEnd() {
        isLunging = false;
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
        zombiePath.SetActive(true);   
    }
}
