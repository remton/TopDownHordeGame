using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : BasicZombieAI
{
    public GameObject owner;
    private bool setEvent = false;

    protected override void Start() {
        base.Start();
        if(owner != null) {
            owner.GetComponent<ZombieHealth>().EventOnDeath += GetComponent<ZombieHealth>().Kill;
            setEvent = true;
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (setEvent) {
            owner.GetComponent<ZombieHealth>().EventOnDeath -= GetComponent<ZombieHealth>().Kill;
        }
    }
}
