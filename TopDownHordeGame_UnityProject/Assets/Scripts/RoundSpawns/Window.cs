using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Window : ZombieSpawn
{ 
    const int TILES_PER_STATE = 3;
    [SerializeField] private HitBoxController playerTrigger;
    private Timer timer;
    private System.Guid boardTimer;

    [SerializeField] private AudioClip breakSound;

    [SerializeField] private int health; // health of the boards on this window
    [SerializeField] private int maxHealth;

    [SerializeField] protected float boardDelay; //Delay between healing boards
    [SerializeField] protected float breakDelay; // the delay in seconds between each zombie hit to the window health
    private bool isOpen = false;
    private bool isBoarding = false;

    public Vector3Int topTile;
    public Vector3Int midTile;
    public Vector3Int bottomTile;

    [Tooltip("list of states for window start with open at index 0. 3 tiles per state")]
    // 3 tiles per state
    //
    // open top,     open mid,     open bottom,
    // 1 board top,  1 board mid,  1 board bottom, 
    // ...
    public List<Tile> Tiles;
    public Tilemap tilemap;

    private void Awake() {
        timer = GetComponent<Timer>();
    }
    private void Start() {
        playerTrigger.EventObjEnter += PlayerEntersTrigger;
        playerTrigger.EventObjExit += PlayerExitsTrigger;
        health = maxHealth;
        UpdateWindowBoards();
    }

    private void Damage(int d) {
        if (isOpen)
            return;
        health -= d;
        if (health <= 0) {
            health = 0;
            AudioManager.instance.PlaySound(breakSound);
            spawnDelay = 0.5F;
            isOpen = true;
        }
        SetHealthRPC(health, maxHealth);
    }

    [Client]
    public void Heal(int h) {
        HealCMD(h);
    }
    [Command(requiresAuthority = false)]
    public void HealCMD(int h) {
        isOpen = false;
        health += h;
        if (health > maxHealth)
            health = maxHealth;
        SetHealthRPC(health, maxHealth);
    }
    [ClientRpc]
    private void SetHealthRPC(int h, int maxH) {
        health = h;
        if (health > maxHealth)
            health = maxHealth;
        UpdateWindowBoards();
    }

    [Server]
    public void FullRepair()
    {
        health = maxHealth;
        isOpen = false;
        UpdateWindowBoards();
    }
    public override bool GetIsOpen() {
        return isOpen;
    }

    private void PlayerEntersTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += StartBoarding;
        player.GetComponent<PlayerActivate>().EventPlayerActivateRelease += StopBoarding;
    }
    private void PlayerExitsTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= StartBoarding;
        player.GetComponent<PlayerActivate>().EventPlayerActivateRelease -= StopBoarding;
        if (isBoarding)
            StopBoarding(player);
    }
    public void StartBoarding(GameObject player) {
        if (!isBoarding) {
            isBoarding = true;
            boardTimer = timer.CreateTimer(boardDelay, Board);
        }
    }
    public void StopBoarding(GameObject player) {
        isBoarding = false;
        timer.KillTimer(boardTimer);
    }
    private void Board() {
        if (isBoarding) {
            timer.CreateTimer(boardDelay, Board);
            Heal(1);
            Debug.Log("Play Board Sound now!");
        }
    }
    private void Update() {
        if (isOpen && numInQueue > 0)
        {
            //spawns a zombie for every zombie in the queue
            if (timeUntilSpawn > 0){
                timeUntilSpawn -= Time.deltaTime;
            }
            else{
                SpawnZombie();
                timeUntilSpawn = spawnDelay;
                numInQueue--;
            }
        }
        else{
            if (timeUntilNextBreak <= 0){
                timeUntilNextBreak = breakDelay;
                //damage the window if a zombie in the queue
                if (!isOpen && numInQueue > 0)
                    Damage(1);
            }
            timeUntilNextBreak -= Time.deltaTime;
        }
    }
    
    private void UpdateWindowBoards() {
        int numStates = ((Tiles.Count) / TILES_PER_STATE);
        int currState = Mathf.CeilToInt(((float)health / maxHealth) * (numStates-1));
        //Debug.Log("State" + currState.ToString());
        int topIndex = currState * TILES_PER_STATE;
        tilemap.SetTile(topTile, Tiles[topIndex]);
        tilemap.SetTile(midTile, Tiles[topIndex+1]);
        tilemap.SetTile(bottomTile, Tiles[topIndex+2]);
    }

    private void SpawnZombie() {
        GameObject zombie = RoundController.instance.CreateZombie();
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}
