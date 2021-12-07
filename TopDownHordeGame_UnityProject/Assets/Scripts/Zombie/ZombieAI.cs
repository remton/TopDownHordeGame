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

    public float playerDistForLunge;
    [SerializeField] private float lungeCooldown;
    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;

    [SerializeField] private float timeBetweenTargetChecks;
    private float timeUntilCheckTarget;

    public void SetValues(int newHealth, float newSpeed, int newDamage) {
        zombieHealth.SetMaxHealth(newHealth);
        zombiePath.target = target;
    }

    //Sets the target to the closeset player
    private void FindTarget(List<GameObject> players) {
        GameObject closest = players[0];
        float closestDist = Vector2.Distance(players[0].transform.position, transform.position);
        for (int i = 0; i < players.Count; i++) {
            if(Vector2.Distance(players[i].transform.position, transform.position) < closestDist) {
                closest = players[i];
                closestDist = Vector2.Distance(players[i].transform.position, transform.position);
            }
        }
        target = closest;
        zombiePath.target = closest;
    }

    private void Awake() {
        zombieLunge = GetComponent<ZombieLunge>();
        zombiePath = GetComponent<ZombiePathfind>();
        zombieHealth = GetComponent<ZombieHealth>();
        timeUntilCheckTarget = timeBetweenTargetChecks;
    }

    private void Start() {
        zombiePath.Activate(2*Time.deltaTime);
        PlayerManager.instance.EventActivePlayersChange += FindTarget;
        FindTarget(PlayerManager.instance.GetActivePlayers());
    }

    private void Update() {
        //zombie lunges
        if (target!=null && !lungeOnCooldown && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            zombiePath.SetActive(false);
            Vector2 dir = target.transform.position - transform.position;
            zombieLunge.Lunge(dir);
        }

        //Lunge cooldown management
        if (lungeOnCooldown) {
            timeUntilLungeCooldown -= Time.deltaTime;
            if(timeUntilLungeCooldown <= 0)
                lungeOnCooldown = false;
        }

        //Update target coundown
        if(timeUntilCheckTarget <= 0) {
            timeUntilCheckTarget = timeBetweenTargetChecks;
            FindTarget(PlayerManager.instance.GetActivePlayers());
        }
    }

    public void OnLungeEnd() {
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
        zombiePath.SetActive(true);   
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= FindTarget;
    }
}
