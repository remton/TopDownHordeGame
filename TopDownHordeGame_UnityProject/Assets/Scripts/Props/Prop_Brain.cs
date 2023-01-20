using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Brain : Prop
{
    public float health;
    public Sprite brokeSprite;
    public AudioClip squelch;
    private ParticleSystem particle;
    private bool hasBroke;
    private void Awake() {
        particle = GetComponent<ParticleSystem>();
    }
    public void BreakBrain() {
        hasBroke = true;
        GetComponent<SpriteRenderer>().sprite = brokeSprite;
        AudioManager.instance.PlaySound(squelch);
    }
    protected override void OnShot(Weapon weapon) {
        if (!hasBroke) {
            health -= weapon.GetDamage();
            if(health <= 0) {
                BreakBrain();
            }
        }
        particle.Play();
    }
}
