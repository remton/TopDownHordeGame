using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZombieSpawn: MonoBehaviour
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

    private void SpawnZombie() {
        GameObject zombie = RoundController.instance.CreateZombie();
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, zombie.transform.position.z);
    }
}
