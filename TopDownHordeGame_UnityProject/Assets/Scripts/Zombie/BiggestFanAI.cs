using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggestFanAI : ZombieAI
{
    private Animator animator;
    private ZombieLunge zombieLunge;
    [SerializeField] private GameObject explosionObj;
    [SerializeField] private float knockbackStrength;
    public float playerDistForLunge;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
        zombieLunge = GetComponent<ZombieLunge>(); 
        zombieLunge.EventLungeEnd += zombieHealth.Kill;
        zombieLunge.EventPrelungeEnd += OnPrelungeEnd;
        zombieHealth.EventOnDeath += Explode;
    }

    public override void SetValues(float newHealth, float newSpeed, float newDamage) {
        base.SetValues(Mathf.CeilToInt(newHealth/2.0f), newSpeed * 1.2f, newDamage);
    }

    protected override void Update() {
        if (isGamePaused)
            return;

        //Lunge then explode!
        if (target != null && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            if (zombieLunge.StartPrelunge(dir)) {
                animator.SetBool("isInPrelunge", true);
            }
            
            Debug.Log("Lunging in " + dir.ToString());
        }
    }
    private void OnPrelungeEnd() {
        animator.SetBool("isInPrelunge", false);
    }

    private void Explode() {
        List<string> damageTags = new List<string>();
        damageTags.Add("Player");
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("Player");
        knockbackTags.Add("ZombieDamageHitbox");

        GameObject obj = Instantiate(explosionObj, transform.position, Quaternion.identity);
        obj.GetComponent<Explosion>().Init(gameObject, damageTags, knockbackTags, damage, knockbackStrength);
    }
}
