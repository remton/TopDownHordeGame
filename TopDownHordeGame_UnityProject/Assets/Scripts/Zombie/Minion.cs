using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : BasicZombieAI
{
    [HideInInspector] public GameObject owner;
    private bool useDeathEvent = false;

    protected override void Start() {
        base.Start();
        if(owner != null) {
            owner.GetComponent<ZombieHealth>().EventOnDeath += GetComponent<ZombieHealth>().Kill;
            useDeathEvent = true;
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (owner != null && useDeathEvent) {
            owner.GetComponent<ZombieHealth>().EventOnDeath -= GetComponent<ZombieHealth>().Kill;
        }
    }
}
