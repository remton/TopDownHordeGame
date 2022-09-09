using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ElevatorSpawnController : NetworkBehaviour {
    // --- Static Vars ---
    public static ElevatorSpawnController instance;

    // --- Values ---
    public bool gameStarted { get; internal set; }
    public int round { get; internal set; }
    public int startRound;
    private float spawnDelay;
    public int maxActiveZombies;

    // --- Serialized Components ---
    [SerializeField] private List<ElevatorZombieSpawn> elevatorZombieSpawns;
    [SerializeField] private float healthCoefficient;

    // --- Prefabs ---
    public List<RandomChoice> zombieList;

    // --- Other private vars
    private List<ElevatorZombieSpawn> activeSpawns = new List<ElevatorZombieSpawn>();
    private int numberActiveZombies;
    private float timeUntilNextSpawn = 10;
    private int numPlayers;
    
    // --- Public Events ---
    public delegate void RoundChange(int round);
    public event RoundChange EventRoundChange;

    // ----- Public Methods -----

    /// <summary> Starts the first round </summary>
    public void StartGame() {
        if (gameStarted)
            return;
        gameStarted = true;
        numPlayers = GameSettings.instance.numPlayers;
        spawnDelay = GetSpawnDeley();
        round = startRound - 1;
        ActivateSpawns(elevatorZombieSpawns);
    }

    /// <summary> Activates the given list of zombie spawnpoints </summary>
    public void ActivateSpawns(List<ElevatorZombieSpawn> spawns) {
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

    /// <summary> Returns all activated windows </summary>
    // public List<Window> GetActiveWindows() {
    //     List<Window> windows = new List<Window>();
    //     foreach (var spawn in activeSpawns) {
    //         if (spawn is Window) {
    //             windows.Add(spawn as Window);
    //         }
    //     }
    //     return windows;
    // }

    /// <summary> [Server] Creates and returns a new zombie </summary>
    [Server]
    public GameObject CreateZombie() {
        //spawn special zombie
        GameObject zombieObj = Instantiate(RandomChoice.ChooseRandom(this.zombieList));
        zombieObj.GetComponent<ZombieAI>().SetValues(GetHealth(), GetSpeed(), GetDamage());
        return zombieObj;
    }

    /// <summary> Called when a zombie dies </summary>
    public void OnZombieDies() {
        numberActiveZombies--;
        if (numberActiveZombies < 0)
            numberActiveZombies = 0;
    }

    public void IncreaseActiveZombies(int numExtra = 1) { // Used when extra zombies are added (such as when the splitter splits)
        for(int i = 0; i < numExtra; i++)
        numberActiveZombies ++; 
    }

    // ----- Private Methods -----

    // --- Round System ---
    #region Round System

    /// <summary> [Server] Sets the next round </summary>
    [Server]
    private void NextRound() {
        round = RoundController.instance.round;
        spawnDelay = GetSpawnDeley();
    }
    
    /// <summary> Updates zombie spawning by 1 frame </summary>
    private void SpawningUpdate() {
            if (timeUntilNextSpawn <= 0) {
                int i = Mathf.RoundToInt(Random.Range(0, activeSpawns.Count));
                activeSpawns[i].AddZombiesToQueue(1); // the window handles spawning the zombie
                numberActiveZombies++;
                timeUntilNextSpawn = spawnDelay;
            }
            else {
                timeUntilNextSpawn -= Time.deltaTime;
            }
    }
    #endregion

    // --- Calculated values ---
    #region Calculated values

    //Returns how many zombies per second to spawn
    private float GetSpawnDeley() {
        //e^(-0.25*(x-7.2)) + 0.3                                                         |This has not been used for a long time. Not sure exactly how long. 
        //e^(-.25(ROUND-7.2))    +    .9    *    1.35^(.1)    /    1.35^(PLAYERCOUNT))    |This equation is too fast. 
        //e^(-.25(ROUND-7.5))    +    .6    *    1.35^(1.9)    /    1.35^(PLAYERCOUNT))   |This is currently being tested. 
        return (((Mathf.Exp(-0.25f * (round - 7.5F)) + 0.6f) * Mathf.Pow(1.35F, 1.9F)) / Mathf.Pow(1.35F, numPlayers));
    }

    private float GetSpeed() {
        return (RoundController.instance.GetSpeed() * .9f); // Zombies from the elevator are slightly slower.
    }
    private float GetHealth() {
        return (RoundController.instance.GetHealth() * healthCoefficient); // Zombies from the elevator are damaged. 
    }
    private float GetDamage() {
        return (RoundController.instance.GetDamage());
    }
    #endregion

    //Called immediatly when script object is created
    private void Awake() {
        gameStarted = false;
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
            Debug.LogWarning("Two instances of elevator spawn controller active! One is being destroyed!");
        }
    }
    //Called when gameobject is destroyed
    private void OnDestroy() {
        if (MyNetworkManager.instance != null) {
            MyNetworkManager.instance.ServerEvent_AllClientsReady -= StartGame;
        }
    }
    //Called on first frame in scene
    private void Start() {
        if (isServer) {
            MyNetworkManager.instance.ServerEvent_AllClientsReady += StartGame;
            if (MyNetworkManager.instance.AllClientsReady())
                StartGame();
        }
        if (!isServer) {
            activeSpawns.Clear();
            this.enabled = false;
            return;
        }
    }
    //Called every frame
    private void Update() {
        if (isServer) {
            SpawningUpdate();
            NextRound();
        }
    }
}