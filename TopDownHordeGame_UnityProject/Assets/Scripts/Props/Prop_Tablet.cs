using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Tablet : Prop
{
    [SerializeField]
    private HitBoxController trigger;
    private Animator animator;

    private bool hasTurnedOn = false;
    public AudioClip turnOnSoundSound;
    public float turnOnSoundVolume;

    private void Awake() {
        animator = GetComponent<Animator>();
        trigger.EventObjEnter += PlayerEnterTrigger;
        trigger.EventObjExit += PlayerExitTrigger;
    }

    private void TurnOn(GameObject player) {
        if (hasTurnedOn)
            return;
        hasTurnedOn = true;
        AudioManager.instance.PlaySound(turnOnSoundSound, turnOnSoundVolume);
        animator.SetTrigger("turnOn");
    }

    private void PlayerEnterTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += TurnOn;
    }
    private void PlayerExitTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= TurnOn;
    }
}
