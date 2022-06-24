using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class Prop_Computer : Prop
{
    [SerializeField]
    private HitBoxController trigger;
    private Animator animator;
    private Timer timer;

    private bool isBroke = false;
    private bool hasTyped = false;
    public float timeBetweenSparks;
    public AudioClip breakSound;
    public AudioClip typeSound;


    private void Awake() {
        timer = GetComponent<Timer>();
        animator = GetComponent<Animator>();
        trigger.EventObjEnter += PlayerEnterTrigger;
        trigger.EventObjExit += PlayerExitTrigger;
    }

    protected override void OnShot() {
        if (isBroke)
            return;

        isBroke = true;
        animator.SetTrigger("shot");
        timer.CreateTimer(timeBetweenSparks, Spark);
        AudioManager.instance.PlaySound(breakSound);
    }

    private void Spark() {
        if (!isBroke)
            return;
        animator.SetTrigger("spark");
        timer.CreateTimer(timeBetweenSparks, Spark);
    }

    private void Type(GameObject player) {
        if (isBroke || hasTyped)
            return;
        hasTyped = true;
        AudioManager.instance.PlaySound(typeSound);
        animator.SetTrigger("type");
    }

    private void PlayerEnterTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += Type;
    }
    private void PlayerExitTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= Type;
    }
}
