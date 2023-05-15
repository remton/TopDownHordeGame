using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Minion : BasicZombieAI
{
    [HideInInspector] public GameObject owner;
    private bool useDeathEvent = false;
    [SerializeField] private GameObject item;

    protected override void Start() {
        base.Start();
        if(owner != null) {
            owner.GetComponent<ZombieHealth>().EventOnDeath += GetComponent<ZombieHealth>().Kill;
            useDeathEvent = true;
        }
        if (isServer)
            DecideItem();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (owner != null && useDeathEvent) {
            owner.GetComponent<ZombieHealth>().EventOnDeath -= GetComponent<ZombieHealth>().Kill;
        }
    }

    [Server]
    private void DecideItem() {
        if (item == null)
            return;
        int rand = Random.Range(0, 5);
        bool hasFlag = (rand == 0);
        SetItemActive(hasFlag);
    }

    [ClientRpc]
    private void SetItemActive(bool b) {
        item.SetActive(b);
    }
}
