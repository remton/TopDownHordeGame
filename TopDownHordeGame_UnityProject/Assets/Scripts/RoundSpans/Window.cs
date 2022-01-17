using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Window : MonoBehaviour
{
    //used and set by roundcontroller to determine which windows are active
    [HideInInspector] public bool isActive;

    [SerializeField] private float breakDelay; // the delay in seconds between each zombie hit to the window health
    private float timeUntilNextBreak;

    private float spawnDelay;
    private float timeUntilSpawn;

    [SerializeField] private int health; // health of the boards on this window
    [SerializeField] private int maxHealth;

    [SerializeField] private bool isOpen = false;
    public bool canSpawn = false;
    private int numInQueue = 0;

    public Vector3Int topTile;
    public Vector3Int midTile;
    public Vector3Int bottomTile;

    public Tilemap tilemap;
    public Tile boardedTop;
    public Tile boardedMid;
    public Tile boardedBottom;
    public Tile openTop;
    public Tile openMid;
    public Tile openBottom;

    //Adds a zombie to spawn
    public void AddZombiesToQueue(int numZombies) {
        numInQueue += numZombies;
    }

    private void Damage(int d) {
        if (health <= 0) {
            SetWindowOpen();
        }
        else health -= d;
    }
    public void Heal(int h) {
        health += h;
        SetWindowBoarded();
    }
    public void FullRepair()
    {
        health = maxHealth;
        SetWindowBoarded();
    }
    public bool GetIsOpen() {
        return isOpen;
    }

    private void Update() {
        if (isOpen && numInQueue > 0)
        {
            //spawns a zombie for every zombie in the queue
            //            for (; numInQueue > 0; numInQueue--) {
            //                timeUntilSpawn = spawnDelay;
            if (timeUntilSpawn > 0)
            {
                timeUntilSpawn -= Time.deltaTime;
            }
            else
            {
                SpawnZombie();
                timeUntilSpawn = spawnDelay;
                numInQueue--;
            }
        }
        //      }
        else
        {
            if (timeUntilNextBreak <= 0)
            {
                timeUntilNextBreak = breakDelay;
                //damage the window for each zombie in the queue
                for (int i = 0; i < numInQueue; i++)
                {
                    if (health <= 0)
                    {
                        SetWindowOpen();
                    }
                    else
                    {
                        Damage(1);
                    }
                }
            }
            timeUntilNextBreak -= Time.deltaTime;
        }
    }
    
    private void SetWindowBoarded() {
//        Debug.Log("CLOSE WINDOW");
        isOpen = false;
        tilemap.SetTile(topTile, boardedTop);
        tilemap.SetTile(midTile, boardedMid);
        tilemap.SetTile(bottomTile, boardedBottom);
    }

    private void SetWindowOpen() {
//        Debug.Log("OPEN WINDOW");
        spawnDelay = .05F;
        isOpen = true;
        tilemap.SetTile(topTile, openTop);
        tilemap.SetTile(midTile, openMid);
        tilemap.SetTile(bottomTile, openBottom);
    }


    private void SpawnZombie() {
        GameObject zombie = RoundController.instance.CreateZombie();
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, zombie.transform.position.z);
    }
}
