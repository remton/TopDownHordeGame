using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Explosion : MonoBehaviour
{
    private float aftershockTime = 0.075f;
    private Timer timer;
    public float timeActive;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float screenShakeIntensity;
    private HitBoxController hitBox;
    private GameObject owner;
    private string ownerTag;
    private List<string> damageTags;
    private List<string> knockbackTags;
    private float damage;
    private float knockbackStrength;

    private void Awake() {
        timer = GetComponent<Timer>();
    }

    private void Start() {
//        SoundPlayer.Play(explosionSound, transform.position);
        AudioManager.instance.PlaySound(explosionSound);
        CameraController.instance.Shake(screenShakeIntensity);
        timer.CreateTimer(aftershockTime, AftershockShake);
    }


    public void Init(GameObject nOwner, List<string> nDamageTags, List<string> nKnockbackTags, float nDamage, float nKnockbackStrength) {
        if (!PlayerConnection.myConnection.isServer) {
            return;
        }

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

    public void DamageActor(GameObject actorHitbox) {
        if(actorHitbox.tag == "Player") {
            actorHitbox.GetComponent<PlayerHealth>().DamageCMD(damage);
        }
        if(actorHitbox.tag == "ZombieDamageHitbox") {
            GameObject zombie = actorHitbox.GetComponent<DamageHitbox>().owner;
            zombie.GetComponent<ZombieHealth>().DamageCMD(damage, owner);
        }
    }
    public void KnockbackActor(GameObject actor) {
        if (actor.HasComponent<DamageHitbox>()){
            actor = actor.GetComponent<DamageHitbox>().owner;
        }
        if (!actor.HasComponent<Rigidbody2D>())
            return;
        Vector2 throwDirection = actor.transform.position - transform.position;
        throwDirection.Normalize();
        if (actor.tag == "Player") {
            actor.GetComponent<PlayerMovement>().KnockBack(knockbackStrength, throwDirection);
            return;
        }
        Vector2 throwAmount = throwDirection * knockbackStrength;
        actor.GetComponent<Rigidbody2D>().AddForce(throwAmount);
    }

    public void AftershockShake() {
        CameraController.instance.Shake(screenShakeIntensity*0.8f);
    }
}
