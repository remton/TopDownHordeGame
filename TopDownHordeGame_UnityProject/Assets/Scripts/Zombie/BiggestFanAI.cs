using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggestFanAI : ZombieAI
{
    private ZombieLunge zombieLunge;
    [SerializeField] private GameObject explosionObj;
    [SerializeField] private float knockbackStrength;
    public float playerDistForLunge;

    protected override void Awake() {
        base.Awake();
        zombieLunge = GetComponent<ZombieLunge>();
    }

    public override void SetValues(int newHealth, float newSpeed, int newDamage) {
        base.SetValues(Mathf.CeilToInt(newHealth/2.0f), newSpeed * 1.2f, newDamage);
        zombieLunge.EventLungeEnd += zombieHealth.Kill;
        zombieHealth.EventOnDeath += Explode;
    }

    protected override void Update() {
        //Lunge then explode!
        if(target != null && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            zombieLunge.Lunge(dir);
        }
    }

    private void Explode() {
        List<string> damageTags = new List<string>();
        damageTags.Add("Player");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("Zombie");
        knockbackTags.Add("Player");

        GameObject obj = Instantiate(explosionObj, transform.position, Quaternion.identity);
        obj.GetComponent<Explosion>().Init(gameObject, damageTags, knockbackTags, damage, knockbackStrength);
    }
}
