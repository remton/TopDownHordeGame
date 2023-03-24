using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Tube : Prop
{
    public SpriteRenderer spriteRenderer;
    public Sprite brokenSprite;
    public Sprite brokenFluidSprite;
    public Sprite subjectSprite;
    public AudioClip breakSound;
    public float breakSoundVolume;
    public GameObject fluidObj;
    public GameObject subjectObj;
    public bool hasSubject;
    public bool broken;
    public float health;

    private ParticleSystem particle;
    private void Awake() {
        particle = GetComponent<ParticleSystem>();
    }
    private void Start() {
        if (hasSubject) {
            subjectObj.GetComponent<SpriteRenderer>().sprite = subjectSprite;
            subjectObj.SetActive(true);
        }
        else {
            subjectObj.SetActive(false);
        }
        if (broken) {
            BreakTube();
        }
    }

    private void BreakTube() {
        canBeShot = false;
        AudioManager.instance.PlaySound(breakSound, breakSoundVolume);
        spriteRenderer.sprite = brokenSprite;
        fluidObj.GetComponent<SpriteRenderer>().sprite = brokenFluidSprite;
        subjectObj.SetActive(false);
    }

    protected override void OnShot(Weapon weapon) {
        particle.Play();
        health -= weapon.GetDamage();
        if(health <= 0) {
            BreakTube();
        }
    }
}
