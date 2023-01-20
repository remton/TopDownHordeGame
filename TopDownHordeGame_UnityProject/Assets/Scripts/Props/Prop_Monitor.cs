using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class Prop_Monitor : Prop
{
    [SerializeField]
    private HitBoxController trigger;
    private Animator animator;
    private Timer timer;

    [SerializeField]
    private float sparkDelay;
    private bool canSpark = false;
    private bool isBroke = false;
    private bool hasTurnedOn = false;
    public AudioClip breakSound;
    public AudioClip turnOnSoundSound;

    private void Awake() {
        timer = GetComponent<Timer>();
        animator = GetComponent<Animator>();
        trigger.EventObjEnter += PlayerEnterTrigger;
        trigger.EventObjExit += PlayerExitTrigger;
    }

    protected override void OnShot(Weapon weapon) {
        if (isBroke) {
            if(canSpark)
                Spark();
            return;
        }
        canSpark = true;
        isBroke = true;
        animator.SetTrigger("break");
        AudioManager.instance.PlaySound(breakSound);
    }

    private void Spark() {
        if (!isBroke)
            return;
        canSpark = false;
        animator.SetTrigger("spark");
        timer.CreateTimer(sparkDelay, AllowSpark);
    }
    private void AllowSpark() {
        canSpark = true;    
    }

    private void TurnOn(GameObject player) {
        if (isBroke || hasTurnedOn)
            return;
        hasTurnedOn = true;
        AudioManager.instance.PlaySound(turnOnSoundSound);
        animator.SetTrigger("turnOn");
    }

    private void PlayerEnterTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TurnOn;
    }
    private void PlayerExitTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TurnOn;
    }
}
