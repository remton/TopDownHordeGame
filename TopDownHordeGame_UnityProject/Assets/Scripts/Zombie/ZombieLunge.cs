using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZombieLunge : MonoBehaviour
{
    [SerializeField] private HitBoxController hitBox;
    [SerializeField] private float waitTime;
    [SerializeField] private float distance;
    [SerializeField] private float lungeTime;

    [SerializeField] private int damage;
    [SerializeField] private float lungeSpeed;

    private bool isLunging = false;
    private bool isWaitingToLunge = false;
    private Vector2 dir;
    private Vector2 targetPos;
    private float timeUntilWaitOver;
    private float timeUntilLungeOver;
    
    private ZombieAI AI;
    private Rigidbody2D rb;

    private void Awake() {
        AI = GetComponent<ZombieAI>();
        rb = GetComponent<Rigidbody2D>();
        hitBox.SetActive(false);
        hitBox.EventObjEnter += Damage;
    }

    private void Damage(GameObject player) {
        player.GetComponent<PlayerHealth>().Damage(damage);
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void Lunge(Vector2 d) {

        if (AI == null) {
            Debug.LogError("Cannot call Lunge before setting the zombie AI");
            return;
        }

        if (isWaitingToLunge || isLunging)
            return;
        dir = d;
        targetPos = transform.position;
        targetPos += dir * distance;
        StartWait();
    }

    void FixedUpdate() {
        if (isWaitingToLunge) {
            timeUntilWaitOver -= Time.fixedDeltaTime;
            if (timeUntilWaitOver <= 0)
                WaitOver();
        }
        if (isLunging) {
            timeUntilLungeOver -= Time.fixedDeltaTime;
            Vector2 newPos = transform.position;
            newPos += dir * lungeSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            if (timeUntilLungeOver <=0) {
                EndLunge();
            }   
        }
    }

    private void StartWait() {
        isWaitingToLunge = true;
        timeUntilWaitOver = waitTime;
    }

    private void WaitOver() {
        isWaitingToLunge = false;
        StartLunge();
    }

    private void StartLunge() {
        timeUntilLungeOver = lungeTime;
        isLunging = true;
        hitBox.SetActive(true);
        hitBox.ForceEntry();
    }

    private void EndLunge() {
        isLunging = false;
        hitBox.SetActive(false);
        AI.OnLungeEnd();
    }

    private void OnDestroy() {
        hitBox.EventObjEnter -= Damage;
    }
}
