using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public bool givesMoney;

    public GameObject target;
    private ZombiePathfind zombiePath;
    protected ZombieHealth zombieHealth;

    protected float speed = 1;
    protected int damage = 1;

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
        zombiePath = GetComponent<ZombiePathfind>();
        zombieHealth = GetComponent<ZombieHealth>();
        timeUntilCheckTarget = timeBetweenTargetChecks;
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
    }

    private void OnDestroy() {
        PlayerManager.instance.EventActivePlayersChange -= FindTarget;
        PauseManager.instance.EventPauseStateChange -= OnPauseStateChange;
    }
}
