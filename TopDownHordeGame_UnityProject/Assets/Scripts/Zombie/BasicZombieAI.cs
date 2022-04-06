using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombieAI : ZombieAI
{
    private Animator animator;
    private ZombieLunge zombieLunge;
    public float playerDistForLunge;
    [SerializeField] private float lungeCooldown;
    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
        zombieLunge = GetComponent<ZombieLunge>();
        zombieLunge.EventLungeEnd += OnLungeEnd;
        zombieLunge.EventPrelungeEnd += OnPrelungeEnd;
    }

    public override void SetValues(int newHealth, float newSpeed, int newDamage) {
        base.SetValues(newHealth, newSpeed, newDamage);
        zombieLunge.SetDamage(newDamage);
    }

    protected override void Update() {
        base.Update();
        //zombie lunges
        if (target!=null && !lungeOnCooldown && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            if(zombieLunge.StartPrelunge(dir))
                animator.SetBool("isInPrelunge", true);
        }
        //Lunge cooldown management
        if (lungeOnCooldown) {
            timeUntilLungeCooldown -= Time.deltaTime;
            if(timeUntilLungeCooldown <= 0)
                lungeOnCooldown = false;
        }
    }
    public void OnPrelungeEnd() {
        animator.SetBool("isInPrelunge", false);
    }

    public void OnLungeEnd() {
        lungeOnCooldown = true;
        timeUntilLungeCooldown = lungeCooldown;
        StartPathing();
    }
}
