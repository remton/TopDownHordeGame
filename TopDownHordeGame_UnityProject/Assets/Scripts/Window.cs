using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] private float breakDelay; // the delay in seconds between each zombie hit to the window health
    private float timeUntilNextBreak; 

    [SerializeField] private int health; // health of the boards on this window

    private bool isOpen = false;
    private int numInQueue = 0;

    //Adds a zombie to spawn
    public void AddZombiesToQueue(int numZombies) {
        numInQueue += numZombies;
    }

    private void Damage(int d) {
        if (health <= 0)
            isOpen = true;
        else health -= d;
    }
    public void Heal(int h) {
        health += h;
        isOpen = false;
    }
    public bool GetIsOpen() {
        return isOpen;
    }

    private void Update() {
        if (isOpen) {
            //spawns a zombie for every zombie in the queue
            for (; numInQueue>0; numInQueue--) {
                SpawnZombie();
            }
        }
        else {
            if(timeUntilNextBreak <= 0) {
                timeUntilNextBreak = breakDelay;
                //damage the window for each zombie in the queue
                for (int i = 0; i < numInQueue; i++) {
                    if(health <= 0) {
                        isOpen = true;
                    }
                    else {
                        Damage(1);
                    }
                }
            }
            timeUntilNextBreak -= Time.deltaTime;
        }
    }
    
    private void SpawnZombie() {
        Debug.Log("ZOMBIE SPAWNS :D");
        GameObject zombie = RoundController.instance.CreateZombie();
        zombie.transform.position = new Vector3(transform.position.x, transform.position.y, zombie.transform.position.z);
    }
}
