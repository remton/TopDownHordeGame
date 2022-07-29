using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombieAI : ZombieAI
{
    [SerializeField] private Animator animator;
    private ZombieLunge zombieLunge;
    public float playerDistForLunge;
    [SerializeField] private float lungeCooldown;
    private bool lungeOnCooldown;
    private float timeUntilLungeCooldown;

    protected override void Awake() {
        base.Awake();
        zombieLunge = GetComponent<ZombieLunge>();
        zombieLunge.EventLungeEnd += OnLungeEnd;
        zombieLunge.EventPrelungeEnd += OnPrelungeEnd;
    }

    public override void SetValues(float newHealth, float newSpeed, float newDamage) {
        base.SetValues(newHealth, newSpeed, newDamage);
        zombieLunge.SetDamage(newDamage);
    }

    protected override void OnUpdate() {
        base.OnUpdate();
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
