using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZombieWindowAttack : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private float windowAttackTime;

    private int damage = 1;
    private bool isWaitingToAttack = false;
    private float timeUntilWaitOver;

    private void Damage(GameObject window)
    {
        window.GetComponent<Window>().Damage(damage);
    }

    public void WindowAttack(Window window)
    {
        if (!isWaitingToAttack)
        {
            myWindow = window;
            StartWait();
        }
    }

    void FixedUpdate()
    {
        if (isWaitingToAttack)
        {
            timeUntilWaitOver -= Time.fixedDeltaTime;
            if (timeUntilWaitOver <= 0)
                WaitOver();
        }
    }

    private void StartWait()
    {
        isWaitingToAttack= true;
        timeUntilWaitOver = waitTime;
    }

    private void WaitOver()
    {
        isWaitingToAttack= false;
        Damage(myWindow);
    }
}
