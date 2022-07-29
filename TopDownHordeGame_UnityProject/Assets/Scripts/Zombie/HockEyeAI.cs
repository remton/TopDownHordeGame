using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HockEyeAI : ZombieAI
{
    public float playerDistForThrow;
    public float throwCooldown;
    public float throwForce;
    public GameObject eyePrefab;

    [SerializeField] private Animator animator;

    private bool isWalking;
    private bool canThrow = true;


    public override void SetValues(float newHealth, float newSpeed, float newDamage)
    {
        base.SetValues(Mathf.CeilToInt(newHealth / 2.0f), newSpeed * 1.5f, Mathf.CeilToInt(newDamage * 0.8f));
    }

    protected override void OnUpdate() {
        base.OnUpdate();
        //We are close enough to our target to throw
        if (Vector2.Distance(target.transform.position, transform.position) <= playerDistForThrow) {
            isWalking = false;
            StopPathing();
            //We can throw
            if (canThrow) {
                StartThrow();
                DisableThrowing();
            }
        }
        else{
            isWalking = true;
            StartPathing();
        }
        animator.SetBool("isIdle", !isWalking);
    }

    private void StartThrow() {
        animator.SetTrigger("throw");
    }
    [Server]
    private void Throw() {
        Vector2 dir = target.transform.position - transform.position;
        dir.Normalize();
        GameObject obj = Instantiate(eyePrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(obj);
        obj.GetComponent<HockEyeEye>().Init(dir, damage, throwForce);
        timer.CreateTimer(throwCooldown, EnableThrowing);
    }
    private void DisableThrowing() {
        canThrow = false;
    }
    private void EnableThrowing() {
        canThrow = true;
    }

    protected override void Awake() {
        base.Awake();
        animator.GetComponent<HockEyeAnimHelper>().EventThrow += Throw;
    }
}


