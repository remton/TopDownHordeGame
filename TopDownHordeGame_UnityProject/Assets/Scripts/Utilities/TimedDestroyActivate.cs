using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyActivate : TimedDestroy
{
    public bool waitingToDestroy;
    public bool isActivated;
    // Start is called before the first frame update
    /*public override*/ new void Start()
    {
        base.Start();
        waitingToDestroy = false;
    }
    void Update()
    {
        if (isActivated)
        {
            Debug.Log("Script being destroyed in TimedDestroyActivate.");
            //Destroy(gameObject.GetComponent<TimedDestroy>());

            Destroy(this);
        }
        else if (destroyTime > 0)
        {
            destroyTime -= Time.deltaTime;
        }
        else if (destroyTime <= 0)
        {
            Debug.Log("Object being destroyed in TimedDestroyActivate.");
            Destroy(gameObject);
        }
    }
}
