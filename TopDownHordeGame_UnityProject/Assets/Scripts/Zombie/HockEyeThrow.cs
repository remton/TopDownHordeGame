using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class HockEyeThrow : MonoBehaviour
{
    [SerializeField] private float timeBetweenThrows;
    [SerializeField] private int damage;
    [SerializeField] private float throwForce;
    [SerializeField] GameObject eyePrefab;
    private bool isThrowing = false;
    private bool isWaitingToThrow = false;
    private Vector2 dir;
    private float timeUntilWaitOver;
    private float timeUntilThrowOver;
    private ZombieAI AI;
    private Rigidbody2D rb;
    private void Awake()
    {
        AI = GetComponent<ZombieAI>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Damage(GameObject player)
    {
        player.GetComponent<PlayerHealth>().Damage(damage);
    }
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    public bool Throw(Vector2 d)
    {
        if (AI == null)
        {
            Debug.LogError("Cannot call Throw before setting the zombie AI");
            return false;
        }
        if (isWaitingToThrow || isThrowing)
            return false;
        dir = d;
        Vector3 hockerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject obj = Instantiate(eyePrefab, hockerPos, Quaternion.identity);
        obj.GetComponent<HockEyeEye>().Init(dir, damage, throwForce);
        //Debug.Log("Tried throwing an eye.");
        StartWait();
        return true;
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

