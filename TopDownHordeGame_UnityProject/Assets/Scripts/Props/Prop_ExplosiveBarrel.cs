using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Prop_ExplosiveBarrel : Prop
{
    [SerializeField] private GameObject flames;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Sprite postExplodeSprite;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float explosionWarningTime;
    [SerializeField] private float acceleratedExplosionWarningTime;
    [SerializeField] private float explosionScale;
    [SerializeField] private float knockbackStrength;
    private Timer timer;
    private Animator animator;
    private void Awake() {
        timer = GetComponent<Timer>();
        animator = GetComponent<Animator>();
    }
    private void Damage(float d) {
        health -= d;
        if (health <= 0)
            StartExplodeSequence(explosionWarningTime);
    }

    private void StartExplodeSequence(float warningTime) {
        canBeShot = false;
        flames.SetActive(true);
        timer.CreateTimer(warningTime, Explode);
    }

    //Creates an Explosion Object
    private void Explode() {
        canBeShot = false;
        //Debug.LogWarning("EXPLODE!");
        List<string> damageTags = new List<string>();
        damageTags.Add("PlayerDamageHitbox");
        damageTags.Add("ZombieDamageHitbox");
        damageTags.Add("Prop");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("PlayerDamageHitbox");
        knockbackTags.Add("ZombieDamageHitbox");
        //Create explosion
        GameObject obj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        obj.transform.localScale = obj.transform.localScale * explosionScale;
        obj.GetComponent<Explosion>().Init(gameObject, damageTags, knockbackTags, damage, knockbackStrength);
        //Update visuals
        animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = postExplodeSprite;
        flames.SetActive(false);
    }
    protected override void OnShot(Weapon weapon) {
        Damage(weapon.GetDamage());
        animator.SetTrigger("wiggle");
    }

    protected override void OnExplode(float dmg) {
        health = 0;
        StartExplodeSequence(acceleratedExplosionWarningTime);
    }
}
