using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HockEyeAI : ZombieAI
{
    [SerializeField] private Animator animator;
    private HockEyeThrow hockEyeThrow;
    public float playerDistForThrow;
    private bool canMove = true;
    private float timeToMove;
    [SerializeField] private float timeCountdown;

    protected override void Awake()
    {
        base.Awake();
        hockEyeThrow = GetComponent<HockEyeThrow>();
        hockEyeThrow.EventOnThrow += OnThrow;
    }

    public override void SetValues(float newHealth, float newSpeed, float newDamage)
    {
        base.SetValues(Mathf.CeilToInt(newHealth / 2.0f), newSpeed * 1.5f, Mathf.CeilToInt(newDamage * 0.8f));
        hockEyeThrow.SetDamage(damage);
    }

    private void OnThrow() {
        animator.SetTrigger("throw");
        animator.SetBool("isIdle", true);
        timeToMove = timeCountdown;
        canMove = false;
    }

    protected override void Update()
    {
        if (isGamePaused)
            return;

        //Throw eyes then pathfind.
        if (target != null && Vector2.Distance(target.transform.position, transform.position) <= playerDistForThrow)
        {
            StopPathing();
            Vector2 dir = target.transform.position - transform.position;
            hockEyeThrow.Throw(dir);
        }
        else if (!canMove && timeToMove > 0)
            timeToMove -= Time.deltaTime;
        else if (timeToMove <= 0)
        {
            animator.SetBool("isIdle", false);
            canMove = true;
            StartPathing();
        }
    }
}


