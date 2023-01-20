using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Tube : Prop
{
    public SpriteRenderer spriteRenderer;
    public Sprite brokenSprite;
    public Sprite subjectSprite;
    public AudioClip breakSound;
    public GameObject fluidObj;
    public GameObject subjectObj;
    public bool hasSubject;
    public bool broken;
    public int durability;

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
        AudioManager.instance.PlaySound(breakSound);
        spriteRenderer.sprite = brokenSprite;
        fluidObj.SetActive(false);
        if (subjectObj.activeSelf) {

        }
        subjectObj.SetActive(false);
    }

    protected override void OnShot(Weapon weapon) {
        durability--;
        if(durability <= 0) {
            BreakTube();
        }
    }
}
