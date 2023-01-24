using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class Prop_Beaker : Prop
{
    [SerializeField] private float destroyTime;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private Animator liquidAnimator; 
    public AudioClip breakSound;
    public float soundVolume;
    protected override void OnShot(Weapon weapon) {
        canBeShot = false;
        AudioManager.instance.PlaySound(breakSound, soundVolume);
        myAnimator.SetTrigger("shot");
        liquidAnimator.SetTrigger("shot");
        TimedDestroy td = gameObject.AddComponent<TimedDestroy>();
        td.destroyTime = destroyTime;
    }
}
