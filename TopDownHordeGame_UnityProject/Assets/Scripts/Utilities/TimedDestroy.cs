using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimedDestroy : MonoBehaviour
{
    private Timer timer;
    private Guid timerID = Guid.Empty;
    public float destroyTime;
    private void Awake() {
        timer = GetComponent<Timer>();
    }
    // Start is called before the first frame update
    protected void Start(){
        Debug.Log("Object is being destroyed in TimedDestroy Start() method.");
        timerID = timer.CreateTimer(destroyTime, DestroyObj);
    }
    public void Cancel() {
        if(timerID != Guid.Empty)
            timer.KillTimer(timerID);
    }
    private void DestroyObj() {
        Destroy(gameObject);
    }
    private void OnDestroy() {
        Cancel();
    }
}
