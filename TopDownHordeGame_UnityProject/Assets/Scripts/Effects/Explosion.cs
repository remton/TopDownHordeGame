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
    [SerializeField] private float explosionSoundVolume;
    [SerializeField] private float screenShakeIntensity;
    private HitBoxController hitBox;
    private GameObject owner;
    private List<string> damageTags;
    private List<string> knockbackTags;
    private float damage;
    private float knockbackStrength;

    private void Awake() {
        timer = GetComponent<Timer>();
    }

    private void Start() {
//        SoundPlayer.Play(explosionSound, transform.position);
        AudioManager.instance.PlaySound(explosionSound, explosionSoundVolume);
        CameraController.instance.Shake(screenShakeIntensity);
        timer.CreateTimer(aftershockTime, AftershockShake);
    }


    public void Init(GameObject nOwner, List<string> nDamageTags, List<string> nKnockbackTags, float nDamage, float nKnockbackStrength) {

        owner = nOwner;
        damageTags = nDamageTags;
        knockbackTags = nKnockbackTags;
        damage = nDamage;
        knockbackStrength = nKnockbackStrength;
        hitBox = GetComponent<HitBoxController>();
        hitBox.triggerTags.Clear();
        
        for (int i = 0; i < nDamageTags.Count; i++) {
            if (!hitBox.triggerTags.Contains(nDamageTags[i])) {
                hitBox.triggerTags.Add(nDamageTags[i]);
            }
        }
        for (int i = 0; i < nKnockbackTags.Count; i++) {
            if (!hitBox.triggerTags.Contains(nKnockbackTags[i]))
                hitBox.triggerTags.Add(nKnockbackTags[i]);
        }
        hitBox.EventObjEnter += ActorEnter;
        hitBox.SetActive(true);
        hitBox.ForceEntry();
        //hitBox.EventObjEnter -= ActorEnter;
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
        if (owner == null) {
            owner = new GameObject();
        }

        if (actor.HasComponent<DamageHitbox>()) {
            actor = actor.GetComponent<DamageHitbox>().owner;
        }
        if (actor.tag == "Player") {
            Debug.Log("player in explosion!");
            if (owner == null || owner.tag != "Player" || (owner.tag == "Player" && actor.GetComponent<PlayerHealth>().HasFriendlyFire()))
                actor.GetComponent<PlayerHealth>().DamageCMD(damage);
        }
        if (actor.tag == "Zombie") {
            actor.GetComponent<ZombieHealth>().DamageCMD(damage, owner);
        }
        if(actor.tag == "Prop" && actor.GetComponent<Prop>().canBeShot) {
            Debug.Log("EXPLOSION: PROP");
            actor.GetComponent<Prop>().ExplodeCMD(damage);
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
