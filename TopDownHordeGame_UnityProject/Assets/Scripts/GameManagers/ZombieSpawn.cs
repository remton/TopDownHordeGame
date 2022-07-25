using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;
public class ZombieSpawn: NetworkBehaviour
{
    //used and set by roundcontroller to determine which windows are active
    [HideInInspector] public bool isActive;

    protected float timeUntilNextBreak;

    protected float spawnDelay;
    protected float timeUntilSpawn;

    public bool canSpawn = false;
    protected int numInQueue = 0;

    //Adds a zombie to spawn
    public void AddZombiesToQueue(int numZombies) {
        numInQueue += numZombies;
    }

    public virtual bool GetIsOpen() {
        return true;
    }

    private void Update() {
        if (numInQueue > 0)
        {
            //spawns a zombie for every zombie in the queue
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
    }

    [Server]
    protected virtual void SpawnZombie() {
        GameObject zombie = RoundController.instance.CreateZombie();
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        NetworkServer.Spawn(zombie);
    }
}
