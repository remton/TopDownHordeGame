using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int round;

    [SerializeField] private RoundDisplay display;

    public List<Window> startRoomWindows;
    private List<Window> activeWindows = new List<Window>();
    public List<GameObject> players;

    bool isWaitingForNextRound = false;
    [SerializeField] private float pauseBetweenRounds;
    private float timeUntilRoundStart;

    int zombiesSpawnedThisRound;
    int numberActiveZombies;

    private float spawnDelay;
    private float timeUntilNextSpawn;

    int zombiesToSpawn;
    public int maxActiveZombies;
    float speed;
    int health;
    int damage;

    public static RoundController instance;

    public void ZombieDies() {
        instance.numberActiveZombies--;
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
            Debug.LogWarning("Two instances of round controller active! One is being destroyed!");
        }
        //Initial round values
        zombiesToSpawn = GetMaxZombies();
        speed = GetSpeed();
        health = GetHealth();
        damage = GetDamage();
        display.RoundChange(round);
        spawnDelay = GetSpawnDeley();
        Debug.Log("Round: " + round.ToString());

        ActivateWindows(startRoomWindows);
    }

    private void Update() {


        //Manages round changing
        if(zombiesSpawnedThisRound >= zombiesToSpawn && numberActiveZombies <= 0) {
            isWaitingForNextRound = true;
            timeUntilRoundStart = pauseBetweenRounds;
        }
        if (isWaitingForNextRound) {
            if (timeUntilRoundStart <= Time.deltaTime) {
                isWaitingForNextRound = false;
                NextRound();
            }
        }
        //Zombie Spawning
        if (activeWindows.Count != 0) {
            if (!isWaitingForNextRound) {
                if(timeUntilNextSpawn <= 0) {
                    if (zombiesSpawnedThisRound < zombiesToSpawn && numberActiveZombies < maxActiveZombies) {
                        int i = Mathf.RoundToInt(Random.Range(0, activeWindows.Count));
                        activeWindows[i].AddZombiesToQueue(1); // the window handles spawning the zombie
                        zombiesSpawnedThisRound++;
                        numberActiveZombies++;
                    }
                    timeUntilNextSpawn = spawnDelay;
                }
                else {
                    timeUntilNextSpawn -= Time.deltaTime;
                }
            }
        }
        else {
            Debug.Log("NO ACTIVE WINDOWS!!!! -_-");
        }
    }

    public GameObject CreateZombie() {
        GameObject zombieObj = Instantiate(zombiePrefab);
        ZombieAI zombie = zombieObj.GetComponent<ZombieAI>();
        zombie.SetValues(GetHealth(),GetSpeed(),GetDamage());
        return zombieObj;
    }

    private void NextRound() {
        round++;
        zombiesToSpawn = GetMaxZombies();
        speed = GetSpeed();
        health = GetHealth();
        damage = GetDamage();
        display.RoundChange(round);
        spawnDelay = GetSpawnDeley();
        Debug.Log("Round: " + round.ToString());
    }

    private int GetMaxZombies() {
        if (round > 15)
            return 5 + 8 * 15;
        else {
            return 5 + 8 * round;
        }
    }

    //Returns how many zombies per second to spawn
    private float GetSpawnDeley() {
        return Mathf.Exp(-0.15f * (round-4)) + 0.1f;
    }

    private float GetSpeed() {
        if (round > 10)
            return 1.2f + (3f/10f) * 10;
        else
        {
            return (1.2f + (3f / 10f) * round);
        }
    }
    private int GetHealth() {
        return round+1;
    }
    private int GetDamage() {
        return Mathf.FloorToInt(Mathf.Sqrt(2f * round));
    }

    public void ActivateWindows(List<Window> windows) {
        for (int i = 0; i < windows.Count; i++) {
            if (!windows[i].isActive) {
                windows[i].isActive = true;
                activeWindows.Add(windows[i]);
            }
            else {
                Debug.Log("window: " + windows[i].name + " already active");
            }
        }
    }
}
