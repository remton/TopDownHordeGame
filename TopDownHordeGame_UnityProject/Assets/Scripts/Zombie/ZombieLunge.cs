using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class ZombieLunge : NetworkBehaviour
{
    [SerializeField] private HitBoxController hitBox;
    [Tooltip("Time in prelunge state")]
    [SerializeField] private float waitTime;
    [Tooltip("Time until lunge is considered over")]
    [SerializeField] private float lungeTime;
    [SerializeField] private float damage;
    [SerializeField] private float lungeForce;

    [Tooltip("When false, lunge force varies by distance to target")]
    public bool normalizeDirection;

    private bool isLunging = false;
    private bool isWaitingToLunge = false;
    private Vector2 dir;
    private float timeUntilWaitOver;
    private float timeUntilLungeOver;

    private Timer timer;
    private BasicZombieAI AI;
    private Rigidbody2D rb;

    public delegate void LungeEnd();
    public event LungeEnd EventLungeEnd;

    public delegate void PrelungeEnd();
    public event PrelungeEnd EventPrelungeEnd;

    //Ping pong makes all hits do knockback

    private static bool pingPong = false;
    private static float pingPongKnockback = 0;
    public static void SetPingPong(float knockback)
    {
        if (knockback > 0)
            pingPong = true;
        else
            pingPong = false;
        pingPongKnockback = knockback;
    }

    private void Awake() {
        AI = GetComponent<BasicZombieAI>();
        rb = GetComponent<Rigidbody2D>();
        timer = GetComponent<Timer>(); 
        hitBox.SetActive(false);
        hitBox.EventObjEnter += Damage;
    }

    public void OnPauseChange(bool isPaused) {
        if (isPaused) {
            timer.SavePauseState();
            timer.PauseAll();
        }
        else {
            timer.LoadPauseState();
        }
    }

    [Server]
    private void Damage(GameObject playerHitbox) {
        if (damage == 0)
            return;
        GameObject player = playerHitbox.GetComponent<DamageHitbox>().owner;
        player.GetComponent<PlayerHealth>().DamageCMD(damage);
        if (pingPong)
        {
            player.GetComponent<PlayerMovement>().KnockbackCMD(Mathf.Max(pingPongKnockback, rb.velocity.magnitude * 75), dir);
        }
    }

    public void SetDamage(float newDamage){
        damage = newDamage;
    }

    public bool StartPrelunge(Vector2 d) {
        if (EventLungeEnd == null) {
            Debug.LogWarning("ZombieLunge.lunge called without setting a callback!");
        }
        if (isWaitingToLunge || isLunging)
            return false;
        dir = d;
        StartWait();
        return true;
    }

    private void StartWait() {
        isWaitingToLunge = true;
        timer.CreateTimer(waitTime, WaitOver);
    }

    private void WaitOver() {
        isWaitingToLunge = false;
        StartLunge();
    }

    private void StartLunge() {
        //Debug.Log("START OF LUNGE");
        isLunging = true;
        if(normalizeDirection)
            rb.AddForce(dir.normalized * lungeForce);
        else
            rb.AddForce(dir * lungeForce);
        hitBox.SetActive(true);
        hitBox.ForceEntry();
        timer.CreateTimer(lungeTime, EndLunge);
        if(EventPrelungeEnd != null) { EventPrelungeEnd.Invoke(); }
    }

    private void EndLunge() {
        //Debug.Log("END OF LUNGE");
        isLunging = false;
        hitBox.SetActive(false);
        if (EventLungeEnd != null && EventPrelungeEnd !=null) {
            EventLungeEnd.Invoke();
        }
        else {
            Debug.LogWarning("ZombieLunge.EndLunge is not returning any callbacks!");
        }
    }

    private void OnDestroy() {
        hitBox.EventObjEnter -= Damage;
    }
}
