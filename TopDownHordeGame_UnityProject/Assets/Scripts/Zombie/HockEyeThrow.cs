using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HockEyeThrow : NetworkBehaviour
{
    [SerializeField] private float timeBetweenThrows;
    [SerializeField] private float damage;
    [SerializeField] private float throwForce;
    [SerializeField] GameObject eyePrefab;
    private bool isThrowing = false;
    private bool isWaitingToThrow = false;
    private Vector2 dir;
    private float timeUntilWaitOver;
    private float timeUntilThrowOver;
    private ZombieAI AI;
    private Rigidbody2D rb;

    public delegate void OnThrow();
    public event OnThrow EventOnThrow;

    private void Awake()
    {
        AI = GetComponent<ZombieAI>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Damage(GameObject player)
    {
        player.GetComponent<PlayerHealth>().DamageCMD(damage);
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    [Command(requiresAuthority = false)]
    public void Throw(Vector2 d)
    {
        if (AI == null)
        {
            Debug.LogError("Cannot call Throw before setting the zombie AI");
            return;
        }
        if (isWaitingToThrow || isThrowing)
            return;

        dir = d.normalized;
        Vector3 hockerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject obj = Instantiate(eyePrefab, hockerPos, Quaternion.identity);
        NetworkServer.Spawn(obj);
        obj.GetComponent<HockEyeEye>().Init(dir, damage, throwForce);
        //Debug.Log("Tried throwing an eye.");
        StartWait();

        if(EventOnThrow != null) { EventOnThrow.Invoke(); }
    }
    void FixedUpdate()
    {
        if (PauseManager.instance.IsPaused())
            return;

        if (isWaitingToThrow)
        {
            timeUntilWaitOver -= Time.fixedDeltaTime;
            if (timeUntilWaitOver <= 0)
                WaitOver();
        }
        if (isThrowing)
        {
            timeUntilThrowOver -= Time.fixedDeltaTime;
            Vector2 newPos = transform.position;
            newPos += dir * throwForce * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
            if (timeUntilThrowOver <= 0)
            {
                EndThrow();
            }
        }
    }
    private void StartWait()
    {
        isWaitingToThrow = true;
        timeUntilWaitOver = timeBetweenThrows;
    }
    private void WaitOver()
    {
        isWaitingToThrow = false;
    }
    private void EndThrow()
    {
        isThrowing = false;
    }
}

