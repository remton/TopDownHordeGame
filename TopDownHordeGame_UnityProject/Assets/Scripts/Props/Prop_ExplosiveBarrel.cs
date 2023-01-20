using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Prop_ExplosiveBarrel : Prop
{
    [SerializeField] private GameObject flames;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float explosionWarningTime;
    [SerializeField] private float explosionScale;
    [SerializeField] private float knockbackStrength;
    private Timer timer;
    private void Awake() {
        timer = GetComponent<Timer>();
    }
    private void Damage(float d) {
        health -= d;
        if (health <= 0)
            StartExplodeSequence();
    }

    [ClientRpc]
    private void StartExplodeSequence() {
        flames.SetActive(true);
        timer.CreateTimer(explosionWarningTime, Explode);
    }

    //Creates an Explosion Object
    private void Explode() {
        //Debug.LogWarning("EXPLODE!");
        List<string> damageTags = new List<string>();
        damageTags.Add("PlayerDamageHitbox");
        damageTags.Add("ZombieDamageHitbox");
        List<string> knockbackTags = new List<string>();
        knockbackTags.Add("PlayerDamageHitbox");
        knockbackTags.Add("ZombieDamageHitbox");

        GameObject obj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        obj.transform.localScale = obj.transform.localScale * explosionScale;
        obj.GetComponent<Explosion>().Init(gameObject, damageTags, knockbackTags, damage, knockbackStrength);
        StartCoroutine(DestroyNextFrame());
    }
    private IEnumerator DestroyNextFrame() {
        gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
    protected override void OnShot(Weapon weapon) {
        Damage(weapon.GetDamage());
    }
}
