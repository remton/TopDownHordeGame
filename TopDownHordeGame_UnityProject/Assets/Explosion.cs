using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Timer timer;
    public float timeActive;
    [SerializeField] private AudioClip explosionSound;
    private HitBoxController hitBox;
    private GameObject owner;
    private string ownerTag;
    private List<string> damageTags;
    private List<string> knockbackTags;
    private int damage;
    private float knockbackStrength;

    private void Awake() {
        timer = GetComponent<Timer>();
    }

    private void Start() {
        AudioClipPlayer.Play(explosionSound, transform.position);
    }

    public void Init(GameObject nOwner, List<string> nDamageTags, List<string> nKnockbackTags, int nDamage, float nKnockbackStrength) {
        owner = nOwner;
        ownerTag = nOwner.tag;
        damageTags = nDamageTags;
        knockbackTags = nKnockbackTags;
        damage = nDamage;
        knockbackStrength = nKnockbackStrength;
        hitBox = GetComponent<HitBoxController>();
        hitBox.triggerTags.Clear();
        for (int i = 0; i < nDamageTags.Count; i++) {
            hitBox.triggerTags.Add(nDamageTags[i]);
        }
        for (int i = 0; i < nKnockbackTags.Count; i++) {
            hitBox.triggerTags.Add(nKnockbackTags[i]);
        }
        hitBox.SetActive(true);
        hitBox.EventObjEnter += ActorEnter;
        timer.CreateTimer(timeActive, StopHitDetection);
    }

    public void StopHitDetection() {
        hitBox.EventObjEnter -= ActorEnter;
    }

    public void ActorEnter(GameObject actor) {
        if(damageTags.Contains(actor.tag)){
            DamageActor(actor);
        }
        if(knockbackTags.Contains(actor.tag)) {
            KnockbackActor(actor);
        }
    }

    public void DamageActor(GameObject actor) {
        if(actor.tag == "Player") {
            actor.GetComponent<PlayerHealth>().Damage(damage);
        }
        if(actor.tag == "Zombie") {
            actor.GetComponent<ZombieHealth>().Damage(damage);
            if(ownerTag == "Player") {
                owner.GetComponent<PlayerStats>().PayForHit(); // Give the player money for the explosion hitting someone 
                if (actor.GetComponent<ZombieHealth>().isDead())
                    owner.GetComponent<PlayerStats>().AddKill();
            }
        }
        else {
            return;
        }
    }
    public void KnockbackActor(GameObject actor) {
        if (!actor.HasComponent<Rigidbody2D>())
            return;
        Vector2 throwDirection = actor.transform.position - transform.position;
        throwDirection.Normalize();
        Vector2 throwAmount = throwDirection * knockbackStrength;
        actor.GetComponent<Rigidbody2D>().AddForce(throwAmount);
    }
}
