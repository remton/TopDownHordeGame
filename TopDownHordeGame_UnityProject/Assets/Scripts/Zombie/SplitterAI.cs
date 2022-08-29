using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(ZombieLunge))]
public class SplitterAI : ZombieAI
{
    [SerializeField] private Animator animator;
    private ZombieLunge zombieLunge;

    [SerializeField] private GameObject spawn; 
    private float timeUntilSpawn;
    public float playerDistForLunge;

    [SerializeField] private float freezeTime;
    [SerializeField] private float lungeCooldown;
    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;
    [SerializeField] private float timeBetweenSpawns; 
    [SerializeField] private int splitNumber;
    

    public override void SetValues(float newHealth, float newSpeed, float newDamage) {
        zombieHealth.SetMaxHealth(newHealth);
        speed = newSpeed;
        damage = newDamage;
    }

    protected override void Awake() {
        base.Awake();
        zombieLunge = GetComponent<ZombieLunge>();
        zombieLunge.EventPrelungeEnd += OnPrelungeEnd;
        zombieLunge.EventLungeEnd += OnLungeEnd;
        timeUntilSpawn = timeBetweenSpawns;
        zombieHealth.EventOnDeath += Split;
    }

    protected override void OnUpdate() {
        base.OnUpdate();
        //zombie lunges
        if (target != null && !lungeOnCooldown && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            if (zombieLunge.StartPrelunge(dir))
                animator.SetBool("isInPrelunge", true);
        }
    }
    public void OnPrelungeEnd() {
        animator.SetBool("isInPrelunge", false);
    }

    public void OnLungeEnd() {
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
        StartPathing();
        GetComponent<Timer>().CreateTimer(lungeCooldown, OnCooldownOver);
    }
    public void OnCooldownOver() {
        lungeOnCooldown = false;
    }


        //Minion summoning
    public void Split() {
        Debug.Log("Split is called.");
        for (int i = 0; i < splitNumber; i++ ) {
            CreateSplit(); 
            RoundController.instance.IncreaseActiveZombies(); 
        }
    }




    [Server]
    private GameObject CreateSplit() {
        Freeze(freezeTime);
        animator.SetTrigger("summon");
        //spawn special minion zombie
        GameObject minion = Instantiate(spawn);
        //minion.GetComponent<Minion>().owner = this.gameObject;
        minion.transform.position = new Vector3(Random.Range(transform.position.x - .1f, transform.position.x + .1f), Random.Range(transform.position.y - .1f, transform.position.y + .1f), minion.transform.position.z);
        minion.GetComponent<ZombieAI>().SetValues(
            zombieHealth.GetMaxHealth() * (1f/splitNumber),      //health
            speed * Random.Range(1.8f, 2.2F),       //speed
            damage);                          //damage
        NetworkServer.Spawn(minion);
        return minion;
    }

}
