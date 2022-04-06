using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: THIS IS MEANT TO BE USED ALONGSIDE BASIC ZOMBIE AI
public class ZathrakAI : MonoBehaviour
{
    private Animator animator;
    public GameObject target;
    private ZombiePathfind zombiePath;
    protected ZombieHealth zombieHealth;
    [SerializeField] private GameObject spawn; 
    protected float speed = 1;
    protected int damage = 1;
    private float timeUntilSpawn;
    [SerializeField] private float timeBetweenSpawns;

    [SerializeField] private float timeBetweenTargetChecks;
    private float timeUntilCheckTarget;
    private bool isPathing = true;

    protected bool isGamePaused;
    protected bool wasPathingBeforePause = false;
    protected virtual void OnPauseStateChange(bool isPaused) {
        if (isPaused) {
            isGamePaused = true;
            wasPathingBeforePause = isPathing;
            StopPathing();
        }
        else {
            isGamePaused = false;
            FindTarget(PlayerManager.instance.GetActivePlayers());
            if (wasPathingBeforePause)
                StartPathing();
        }
    }

    public virtual void SetValues(int newHealth, float newSpeed, int newDamage) {
        zombieHealth.SetMaxHealth(newHealth);
        speed = newSpeed;
        damage = newDamage;
    }

    //Sets the target to the closeset player
    private void FindTarget(List<GameObject> players) {
        if (players.Count <= 0)
            return;
        GameObject closest = players[0];
        float closestDist = Vector2.Distance(players[0].transform.position, transform.position);
        for (int i = 0; i < players.Count; i++) {
            if (Vector2.Distance(players[i].transform.position, transform.position) < closestDist) {
                closest = players[i];
                closestDist = Vector2.Distance(players[i].transform.position, transform.position);
            }
        }
        target = closest;
        zombiePath.target = closest;
    }

    protected void StartPathing() {
        isPathing = true;
        zombiePath.SetActive(true);
    }
    protected void StopPathing() {
        isPathing = false;
        zombiePath.SetActive(false);
    }

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        zombiePath = GetComponent<ZombiePathfind>();
        zombieHealth = GetComponent<ZombieHealth>();
        timeUntilCheckTarget = timeBetweenTargetChecks;
        timeUntilSpawn = timeBetweenSpawns;
    }

    protected virtual void Start() {
        zombiePath.Activate(2 * Time.deltaTime);
        PlayerManager.instance.EventActivePlayersChange += FindTarget;
        PauseManager.instance.EventPauseStateChange += OnPauseStateChange;
        FindTarget(PlayerManager.instance.GetActivePlayers());
    }

    protected virtual void Update() {
        if (isGamePaused)
            return;

        //Update target coundown
        if (timeUntilCheckTarget <= 0) {
            timeUntilCheckTarget = timeBetweenTargetChecks;
            FindTarget(PlayerManager.instance.GetActivePlayers());
        }
        //Minion summoning
        if (timeUntilSpawn <= 0) {
            timeUntilSpawn = timeBetweenSpawns;
            CreateMinion();
        }
        timeUntilCheckTarget -= Time.deltaTime;
        timeUntilSpawn -= Time.deltaTime;
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= FindTarget;
        PauseManager.instance.EventPauseStateChange -= OnPauseStateChange;
    }
    private GameObject CreateMinion() {
        animator.SetTrigger("summon");
        //spawn special zombie
        GameObject zombieObj = Instantiate(spawn);
        zombieObj.transform.position = new Vector3(transform.position.x, transform.position.y, zombieObj.transform.position.z);
        zombieObj.GetComponent<ZombieAI>().SetValues(Mathf.RoundToInt(zombieHealth.GetMaxHealth() * .3f), speed * Random.Range(1.1f, 1.2F), Mathf.Max(Mathf.RoundToInt(damage * .3F), 1));
        return zombieObj;
    }
}
