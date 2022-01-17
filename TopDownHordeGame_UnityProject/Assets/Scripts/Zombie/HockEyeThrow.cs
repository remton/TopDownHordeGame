using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HockEyeThrow : MonoBehaviour
{
    [SerializeField] private HitBoxController hitBox;
    [SerializeField] private float waitTime;
    [SerializeField] private float distance;
    [SerializeField] private float throwTime;

    [SerializeField] private int damage;
    [SerializeField] private float throwSpeed;

    [SerializeField] GameObject eyePrefab;

    private bool isThrowing = false;
    private bool isWaitingToThrow = false;
    private Vector2 dir;
    private Vector2 targetPos;
    private float timeUntilWaitOver;
    private float timeUntilLungeOver;

    private ZombieAI AI;
    private Rigidbody2D rb;

    private void Awake()
    {
        AI = GetComponent<ZombieAI>();
        rb = GetComponent<Rigidbody2D>();
        hitBox.SetActive(false);
        hitBox.EventObjEnter += Damage;
    }

    private void Damage(GameObject player)
    {
        player.GetComponent<PlayerHealth>().Damage(damage);
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void Lunge(Vector2 d)
    {

        if (AI == null)
        {
            Debug.LogError("Cannot call Lunge before setting the zombie AI");
            return;
        }

        if (isWaitingToThrow || isThrowing)
            return;
        dir = d;
        targetPos = transform.position;
        targetPos += dir * distance;
        StartWait();
    }

    void FixedUpdate()
    {
        if (isWaitingToThrow)
        {
            timeUntilWaitOver -= Time.fixedDeltaTime;
            if (timeUntilWaitOver <= 0)
                WaitOver();
        }
        if (isThrowing)
        {
            timeUntilLungeOver -= Time.fixedDeltaTime;
            Vector2 newPos = transform.position;
            newPos += dir * throwSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            if (timeUntilLungeOver <= 0)
            {
                EndLunge();
            }
        }
    }

    private void StartWait()
    {
        isWaitingToThrow = true;
        timeUntilWaitOver = waitTime;
    }

    private void WaitOver()
    {
        isWaitingToThrow = false;
        StartLunge();
    }

    private void StartLunge()
    {
        timeUntilLungeOver = throwTime;
        isThrowing = true;
        hitBox.SetActive(true);
        hitBox.ForceEntry();
    }

    private void EndLunge()
    {
        isThrowing = false;
        hitBox.SetActive(false);
        AI.OnLungeEnd();
    }

    private void OnDestroy()
    {
        hitBox.EventObjEnter -= Damage;
    }
}
