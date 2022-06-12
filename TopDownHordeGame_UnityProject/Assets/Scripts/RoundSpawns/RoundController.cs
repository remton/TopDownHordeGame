using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundController : NetworkBehaviour
{
    public delegate void RoundChange(int round);
    public event RoundChange EventRoundChange;

    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int round;
    public int GetRound() { return round; }

    [SerializeField] private RoundDisplay display;

    public List<ZombieSpawn> startRoomWindows;
    private List<ZombieSpawn> activeSpawns = new List<ZombieSpawn>();
    public List<GameObject> players;

    bool isWaitingForNextRound = false;
    bool hasShownRoundChange = false;
    [SerializeField] private float pauseBeforeGameStart;
    [SerializeField] private float pauseBetweenRounds;
    private float timeUntilRoundStart;

    int zombiesSpawnedThisRound;
    int numberActiveZombies;
    int numPlayers;

    private float spawnDelay;
    private float timeUntilNextSpawn;

    int zombiesToSpawn;
    public int maxActiveZombies;
    float speed;
    float health;
    float damage;

    private int zombieI;
    public List<RandomChoice> zombieList;

    public static RoundController instance;
    public void ZombieDies() {
        instance.numberActiveZombies--;
        if (instance.numberActiveZombies < 0)
            numberActiveZombies = 0;
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
            Debug.LogWarning("Two instances of round controller active! One is being destroyed!");
        }
    }
    private void Start() {
        if (!isServer) {
            activeSpawns.Clear();
            this.enabled = false;
            return;
        }

        //Initial round values
        numPlayers = GameSettings.instance.numPlayers;
        zombiesToSpawn = GetMaxZombies();
        speed = GetSpeed();
        health = GetHealth();
        damage = GetDamage();
        display.RoundChange(round);
        spawnDelay = GetSpawnDeley();
        //Debug.Log("Round: " + round.ToString());
        //Debug.Log(numPlayers + " players");
        isWaitingForNextRound = true;
        timeUntilRoundStart = pauseBeforeGameStart;
        ActivateSpawns(startRoomWindows);
    }

    private void Update() {
        //Manages round changing
        if (isWaitingForNextRound) {
            if (!hasShownRoundChange) { 
                display.RoundChange(round + 1); //display before the zombies start spawning
                PlayerManager.instance.ReviveDownedPlayers();
                PlayerManager.instance.RespawnDeadPlayers();
                hasShownRoundChange = true;
            }
            if (timeUntilRoundStart <= Time.deltaTime) {
                isWaitingForNextRound = false;
                NextRound();
            }
            timeUntilRoundStart -= Time.deltaTime; 
        }
        else if (zombiesSpawnedThisRound >= zombiesToSpawn && numberActiveZombies <= 0)
        {
            isWaitingForNextRound = true;
            timeUntilRoundStart = pauseBetweenRounds;
        }
        //Zombie Spawning
        if (activeSpawns.Count != 0) {
            if (!isWaitingForNextRound) {
                if(timeUntilNextSpawn <= 0) {
                    if (zombiesSpawnedThisRound < zombiesToSpawn) {
                        int i = Mathf.RoundToInt(Random.Range(0, activeSpawns.Count));
                        activeSpawns[i].AddZombiesToQueue(1); // the window handles spawning the zombie
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

    [Server]
    public GameObject CreateZombie() {
        //spawn special zombie
        GameObject zombieObj = Instantiate(RandomChoice.ChooseRandom(zombieList));
        zombieObj.GetComponent<ZombieAI>().SetValues(GetHealth(), GetSpeed(), GetDamage());
        NetworkServer.Spawn(zombieObj);
        return zombieObj;
    }

    private void NextRound() {
        round++;
        zombiesToSpawn = GetMaxZombies();
        speed = GetSpeed();
        health = GetHealth();
        damage = GetDamage();
        hasShownRoundChange = false;
        spawnDelay = GetSpawnDeley();
        zombiesSpawnedThisRound = 0;
        timeUntilNextSpawn = 0;
        if (EventRoundChange != null) { EventRoundChange.Invoke(round); }
        //Debug.Log("Round: " + round.ToString());
    }

    private int GetMaxZombies() {
        if (round > 15)
            return Mathf.FloorToInt(5 + 8 * 15 * Mathf.Log10(6 + Mathf.Pow(numPlayers, 3) / 2));
        else {
            //Debug.Log((numPlayers + " players in GetMaxZombies"));
            return Mathf.FloorToInt(0 + 8 * round * Mathf.Log10(6 + Mathf.Pow(numPlayers, 3) / 2));
        }
    }
    //Returns how many zombies per second to spawn
    private float GetSpawnDeley() {
        //e^(-0.25*(x-7.2)) + 0.3
        return (((Mathf.Exp(-0.25f * (round-7.2F)) + 0.3f) * 1.35F) / Mathf.Pow(1.35F, numPlayers));
    }

    private float GetSpeed() {
        int calcRound = round;
        if (round > 10)
            calcRound = 10;
        return 1.3f + 3f / 10f * calcRound + Random.Range(-.04F * calcRound, .08F * calcRound); // Gives zombies a random speed
    }
    private float GetHealth() {
        //0.10(0.7round+0.8)^2 + 4 until round 6
        //round+1 after round 6
        if(round < 6 )
            return Mathf.FloorToInt(0.10f * (0.7f * round + 0.8f) * (0.7f * round + 0.8f) + 4);
        else {
            return round + 1;
        }
    }
    private float GetDamage() {
        return Mathf.Sqrt(2f * round) * .75f - .5f;
    }

    public void ActivateSpawns(List<ZombieSpawn> spawns) {
        for (int i = 0; i < spawns.Count; i++) {
            if (!spawns[i].isActive) {
                spawns[i].isActive = true;
                activeSpawns.Add(spawns[i]);
            }
            else {
                Debug.Log("window: " + spawns[i].name + " already active");
            }
        }
    }
    public List<Window> GetActiveWindows() {
        List<Window> windows = new List<Window>();
        foreach (var spawn in activeSpawns) {
            if(spawn is Window) {
                windows.Add(spawn as Window);
            }
        }
        return windows;
    }
}
