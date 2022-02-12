using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZombieLunge : MonoBehaviour
{
    [SerializeField] private HitBoxController hitBox;
    [SerializeField] private float waitTime;
    [SerializeField] private float lungeTime;
    [SerializeField] private int damage;
    [SerializeField] private float lungeForce;

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

    private void Awake() {
        AI = GetComponent<BasicZombieAI>();
        rb = GetComponent<Rigidbody2D>();
        timer = GetComponent<Timer>();
        hitBox.SetActive(false);
        hitBox.EventObjEnter += Damage;
    }

    private void Damage(GameObject player) {
        player.GetComponent<PlayerHealth>().Damage(damage);
    }
    public void SetDamage(int newDamage){
        damage = newDamage;
    }

    public void Lunge(Vector2 d) {
        if (EventLungeEnd == null) {
            Debug.LogWarning("ZombieLunge.lunge called without setting a callback!");
        }
        if (isWaitingToLunge || isLunging)
            return;
        dir = d;
        StartWait();
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
        Debug.Log("START OF LUNGE");
        isLunging = true;
        rb.AddForce(dir * lungeForce);
        hitBox.SetActive(true);
        hitBox.ForceEntry();
        timer.CreateTimer(lungeTime, EndLunge);
    }

    private void EndLunge() {
        Debug.Log("END OF LUNGE");
        isLunging = false;
        hitBox.SetActive(false);
        if (EventLungeEnd != null) {
            EventLungeEnd.Invoke();
        }
        else {
            Debug.LogWarning("ZombieLunge.EndLunge is not returning a callback!");
        }
    }

    private void OnDestroy() {
        hitBox.EventObjEnter -= Damage;
    }
}
