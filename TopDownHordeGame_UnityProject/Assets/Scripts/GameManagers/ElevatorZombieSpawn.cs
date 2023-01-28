using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

[RequireComponent(typeof(Timer))]
public class ElevatorZombieSpawn : ZombieSpawn {

    public List<RandomChoice<GameObject>> zombieList;
    private Timer timer;
    private System.Guid spawnTimerID;
    private bool doSpawns = false;


    private void Start() {
        timer = GetComponent<Timer>();
        if (isServer) {
            RoundController.instance.EventRoundChange += FirstRoundStart;
        }
    }

    public void FirstRoundStart(int round) {
        RoundController.instance.EventRoundChange -= FirstRoundStart;
        StartSpawns();
    }

    public void StartSpawns() {
        doSpawns = true;
        SpawnZombie();
    }

    public void StopSpawns() {
        doSpawns = false;
        timer.KillTimer(spawnTimerID);
    }

    [Server]
    public GameObject CreateZombie() {
        GameObject zombieObj = Instantiate(RandomChoice<GameObject>.ChooseRandom(zombieList));
        zombieObj.GetComponent<ZombieAI>().SetValues(GetHealth(), GetSpeed(), GetDamage());
        return zombieObj;
    }

    [Server]
    protected override void SpawnZombie() {
        if (!doSpawns || RoundController.instance.numberActiveZombies >= RoundController.instance.maxActiveZombies)
            return;
        GameObject zombie = CreateZombie();
        zombie.AddComponent(typeof(DoNotCount));
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        NetworkServer.Spawn(zombie);
        spawnTimerID = timer.CreateTimer(GetSpawnDeley(), SpawnZombie);
    }

    private float GetSpawnDeley() {
        //e^(-0.25*(x-7.2)) + 0.3                                                         |This has not been used for a long time. Not sure exactly how long. 
        //e^(-.25(ROUND-7.2))    +    .9    *    1.35^(.1)    /    1.35^(PLAYERCOUNT))    |This equation is too fast. 
        //e^(-.25(ROUND-7.5))    +    .6    *    1.35^(1.9)    /    1.35^(PLAYERCOUNT))   |This is currently being tested. 
        return (((Mathf.Exp(-0.25f * (RoundController.instance.round - 7.5F)) + 0.6f) * Mathf.Pow(1.35F, 1.9F)) / Mathf.Pow(1.35F, PlayerManager.instance.NumPlayers()));
    }
    private float GetSpeed() {
        return (RoundController.instance.GetSpeed() * .9f); // Zombies from the elevator are slightly slower.
    }
    private float GetHealth() {
        return (RoundController.instance.GetHealth() * 0.7f); // Zombies from the elevator are damaged. 
    }
    private float GetDamage() {
        return (RoundController.instance.GetDamage());
    }
}
