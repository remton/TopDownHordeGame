using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BiggestFanAI : ZombieAI
{
    [SerializeField] private Animator animator;
    private ZombieLunge zombieLunge;
    [SerializeField] private GameObject explosionObj;
    [SerializeField] private float knockbackStrength;
    public float playerDistForLunge;

    protected override void Awake() {
        base.Awake();
        zombieLunge = GetComponent<ZombieLunge>();
    }

    protected override void Start() {
        base.Start();
        if (isServer) {
            //Debug.Log("Subscribe server BIGGESTFAN");
            zombieLunge.EventLungeEnd += zombieHealth.Kill;
            zombieLunge.EventPrelungeEnd += OnPrelungeEnd;
            zombieHealth.EventOnDeath += Explode;
            zombieHealth.DontDestroyOnDeath = true;
        }
    }

    public override void SetValues(float newHealth, float newSpeed, float newDamage) {
        base.SetValues(Mathf.CeilToInt(newHealth*2f), newSpeed * .8f, newDamage * 4.5f);
    }

    protected override void OnUpdate() {
        base.OnUpdate();

        //Lunge then explode!
        if (target != null && Vector2.Distance(target.transform.position, transform.position) <= playerDistForLunge) {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            if (zombieLunge.StartPrelunge(dir)) {
                animator.SetBool("isInPrelunge", true);
            }
            //Debug.Log("Lunging in " + dir.ToString());
        }
    }
    private void OnPrelungeEnd() {
        animator.SetBool("isInPrelunge", false);
    }

    [ClientRpc]
    private void Explode() {
        //Debug.LogWarning("EXPLODE!");

        List<string> damageTags = new List<string>();
        damageTags.Add("PlayerDamageHitbox");
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("PlayerDamageHitbox");
        knockbackTags.Add("ZombieDamageHitbox");

        GameObject obj = Instantiate(explosionObj, transform.position, Quaternion.identity);
        obj.GetComponent<Explosion>().Init(gameObject, damageTags, knockbackTags, damage, knockbackStrength);
        StartCoroutine(DestroyNextFrame());
    }

    private IEnumerator DestroyNextFrame() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

}
