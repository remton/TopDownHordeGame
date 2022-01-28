using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimedDestroy : MonoBehaviour
{
    private Timer timer;
    private int timerID = -1;
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
        if(timerID != -1)
            timer.KillTimer(timerID);
    }
    private void DestroyObj() {
        Destroy(gameObject);
    }
    private void OnDestroy() {
        Cancel();
    }
}
