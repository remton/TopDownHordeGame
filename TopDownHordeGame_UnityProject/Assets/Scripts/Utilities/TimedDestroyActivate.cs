using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyActivate : TimedDestroy
{
    public bool waitingToDestroy;
    public bool isActivated;
    // Start is called before the first frame update
    void start()
    {
        waitingToDestroy = false;
    }
    void update()
    {
        if (destroyTime > 0)
        {
            destroyTime -= Time.deltaTime;
        }
        else if (!waitingToDestroy && !isActivated)
        {
            Destroy(gameObject);
        }
    }
}
