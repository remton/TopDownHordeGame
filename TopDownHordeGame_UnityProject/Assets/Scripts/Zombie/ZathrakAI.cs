using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(ZombieLunge))]
public class ZathrakAI : ZombieAI
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
    }

    protected override void Update() {
        base.Update();
        //zombie lunges
        if (target != null && !lungeOnCooldown && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            if (zombieLunge.StartPrelunge(dir))
                animator.SetBool("isInPrelunge", true);
        }
        //Minion summoning
        if (timeUntilSpawn <= 0) {
            timeUntilSpawn = timeBetweenSpawns;
            CreateMinion();
        }
        timeUntilSpawn -= Time.deltaTime;
    }
    public void OnPrelungeEnd() {
        animator.SetBool("isInPrelunge", false);
    }

    public void OnLungeEnd() {
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
        StartPathing();
    }
    [Server]
    private GameObject CreateMinion() {
        Freeze(freezeTime);
        animator.SetTrigger("summon");
        //spawn special minion zombie
        GameObject minion = Instantiate(spawn);
        minion.GetComponent<Minion>().owner = this.gameObject;
        minion.transform.position = new Vector3(transform.position.x, transform.position.y, minion.transform.position.z);
        minion.GetComponent<ZombieAI>().SetValues(
            zombieHealth.GetMaxHealth() * .2f,      //health
            speed * Random.Range(1.6f, 1.8F),       //speed
            damage * .3F);                          //damage
        NetworkServer.Spawn(minion);
        return minion;
    }

}
