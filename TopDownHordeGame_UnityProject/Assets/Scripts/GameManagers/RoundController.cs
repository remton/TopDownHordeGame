using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundController : NetworkBehaviour {
    // --- Static Vars ---
    public static RoundController instance;

    // --- Values ---
    public bool gameStarted { get; internal set; }
    public int round { get; internal set; }
    public int startRound;
    [SerializeField] private float pauseBeforeGameStart;
    [SerializeField] private float pauseBetweenRounds;
    private float spawnDelay;
    public int maxActiveZombies;

    // --- Serialized Components ---
    [SerializeField] private List<ZombieSpawn> startRoomZombieSpawns;
    [SerializeField] private RoundDisplay display;

    // --- Prefabs ---
    public List<RandomChoice> zombieList;

    // --- Other private vars
    private List<ZombieSpawn> activeSpawns = new List<ZombieSpawn>();
    private int zombiesToSpawn;
    private int zombiesSpawnedThisRound;
    public int numberActiveZombies { get; internal set; }
    private bool isWaitingForNextRound = false;
    private bool hasShownRoundChange = false;
    private float timeUntilRoundStart;
    private float timeUntilNextSpawn;
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
        zombiesToSpawn = GetMaxZombies();
        display.RoundChange(round);
        spawnDelay = GetSpawnDeley();
        round = startRound - 1;
        isWaitingForNextRound = true;
        timeUntilRoundStart = pauseBeforeGameStart;
        ActivateSpawns(startRoomZombieSpawns);
    }

    /// <summary> Activates the given list of zombie spawnpoints </summary>
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

    /// <summary> Returns all activated windows </summary>
    public List<Window> GetActiveWindows() {
        List<Window> windows = new List<Window>();
        foreach (var spawn in activeSpawns) {
            if (spawn is Window) {
                windows.Add(spawn as Window);
            }
        }
        return windows;
    }

    /// <summary> [Server] Creates and returns a new zombie </summary>
    [Server]
    public GameObject CreateZombie() {
        //spawn special zombie
        GameObject zombieObj = Instantiate(RandomChoice.ChooseRandom(zombieList));
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

    /// <summary> [Server] Starts the next round </summary>
    [Server]
    private void NextRound() {
        round++;
        zombiesToSpawn = GetMaxZombies();
        hasShownRoundChange = false;
        spawnDelay = GetSpawnDeley();
        zombiesSpawnedThisRound = 0;
        timeUntilNextSpawn = 0;
        NextRoundRPC(round);
        //Debug.Log("Round: " + round.ToString());
    }

    /// <summary> [Server] Updates round system by 1 frame </summary>
    [Server]
    private void RoundUpdate() {
        if (isWaitingForNextRound) {
            if (!hasShownRoundChange) {
                DisplayRoundChangeRPC(round + 1); //display before the zombies start spawning
                PlayerManager.instance.HealAllPlayersRPC();
                PlayerManager.instance.ReviveDownedPlayersRPC();
                PlayerManager.instance.RespawnDeadPlayersRPC();
                hasShownRoundChange = true;
            }
            if (timeUntilRoundStart <= Time.deltaTime) {
                isWaitingForNextRound = false;
                NextRound();
            }
            timeUntilRoundStart -= Time.deltaTime;
        }
        else if (zombiesSpawnedThisRound >= zombiesToSpawn && numberActiveZombies <= 0) {
            isWaitingForNextRound = true;
            timeUntilRoundStart = pauseBetweenRounds;
        }
    }

    /// <summary> [ClientRpc] Updates round on all clients</summary>
    [ClientRpc]
    private void NextRoundRPC(int newRound) {
        this.round = newRound;
        hasShownRoundChange = false;
        if (EventRoundChange != null) { EventRoundChange.Invoke(newRound); }
    }

    /// <summary> [TargetRpc] Updates round on the client with network</summary>
    [TargetRpc]
    private void NextRoundTRPC(NetworkConnection network, int newRound) {
        this.round = newRound;
        hasShownRoundChange = false;
        if (EventRoundChange != null) { EventRoundChange.Invoke(newRound); }
    }

    /// <summary> [ClientRpc] Shows the round popup for all clients </summary>
    [ClientRpc]
    private void DisplayRoundChangeRPC(int round) {
        display.RoundChange(round);
    }

    /// <summary> [ClientRpc] Shows the round popup for the client with network</summary>
    [TargetRpc]
    private void DisplayRoundChangeTRPC(NetworkConnection network, int round) {
        display.RoundChange(round);
    }
    #endregion

    /// <summary> Called on server when a new player connects to the game </summary>
    [Server]
    private void PlayerConnect(PlayerConnection connection) {
        if (gameStarted) {
            NextRoundTRPC(connection.connectionToClient, round);
            DisplayRoundChangeTRPC(connection.connectionToClient, round);
        }
    }

    /// <summary> Updates zombie spawning by 1 frame </summary>
    private void SpawningUpdate() {
        if (activeSpawns.Count != 0 && !isWaitingForNextRound) {
            if (timeUntilNextSpawn <= 0) {
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

    // --- Calculated values ---
    #region Calculated values
    private int GetMaxZombies() {
        if (round > 15)
            return Mathf.FloorToInt(5 + 8 * 15 * Mathf.Log10(6 + Mathf.Pow(numPlayers, 3) / 2));
        else {
            //Debug.Log((numPlayers + " players in GetMaxZombies"));
            return Mathf.FloorToInt(0 + 8 * round * Mathf.Log10(6 + Mathf.Pow(numPlayers, 3) / 2));
        }
    }
    //Returns how many zombies per second to spawn
    public float GetSpawnDeley() {
        //e^(-0.25*(x-7.2)) + 0.3
        return (((Mathf.Exp(-0.25f * (round - 7.2F)) + 0.3f) * 1.35F) / Mathf.Pow(1.35F, numPlayers));
    }

    public float GetSpeed() {
        int calcRound = round;
        if (round > 10)
            calcRound = 10;
        return 1.3f + 3f / 10f * calcRound + Random.Range(-.04F * calcRound, .08F * calcRound); // Gives zombies a random speed
    }
    public float GetHealth() {
        //0.10(0.7round+0.8)^2 + 4 until round 6
        //round+1 after round 6
        if (round < 6)
            return Mathf.FloorToInt(0.10f * (0.7f * round + 0.8f) * (0.7f * round + 0.8f) + 4);
        else {
            return round + 1;
        }
    }
    public float GetDamage() {
        return Mathf.Sqrt(2f * round) * .75f - .5f;
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
            Debug.LogWarning("Two instances of round controller active! One is being destroyed!");
        }
    }
    //Called when gameobject is destroyed
    private void OnDestroy() {
        if (MyNetworkManager.instance != null) {
            MyNetworkManager.instance.ServerEvent_PlayerConnectionAdded -= PlayerConnect;
            MyNetworkManager.instance.ServerEvent_AllClientsReady -= StartGame;
        }
    }
    //Called on first frame in scene
    private void Start() {
        if (isServer) {
            MyNetworkManager.instance.ServerEvent_PlayerConnectionAdded += PlayerConnect;
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
            RoundUpdate();
            SpawningUpdate();
        }
    }
}
