using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
    public float breakSoundVolume;
    public AudioClip typeSound;
    public float typeSoundVolume;

    public delegate void OnActivate();
    public event OnActivate EventOnActivate;
    public delegate void OnDestroy();
    public event OnDestroy EventOnBreak;

    private void Awake() {
        timer = GetComponent<Timer>();
        animator = GetComponent<Animator>();
        trigger.EventObjEnter += PlayerEnterTrigger;
        trigger.EventObjExit += PlayerExitTrigger;
    }

    protected override void OnShot(Weapon weapon) {
        if (isBroke)
            return;
        isBroke = true;
        animator.SetTrigger("shot");
        timer.CreateTimer(timeBetweenSparks, Spark);
        AudioManager.instance.PlaySound(breakSound, breakSoundVolume);
        if (EventOnBreak != null) { EventOnBreak.Invoke(); }
    }

    private void Spark() {
        if (!isBroke)
            return;
        animator.SetTrigger("spark");
        timer.CreateTimer(timeBetweenSparks, Spark);
    }

    [Client]
    private void Type(GameObject player) {
        if (isBroke || hasTyped)
            return;
        TypeCMD();
    }
    [Command(requiresAuthority = false)]
    private void TypeCMD() {
        TypeRPC();
    }
    [ClientRpc]
    private void TypeRPC() {
        OnType();
    }
    public virtual void OnType() {
        hasTyped = true;
        AudioManager.instance.PlaySound(typeSound, typeSoundVolume);
        animator.SetTrigger("type");
        if (EventOnActivate != null) { EventOnActivate.Invoke(); }
    }


    private void PlayerEnterTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += Type;
    }
    private void PlayerExitTrigger(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= Type;
    }
}
