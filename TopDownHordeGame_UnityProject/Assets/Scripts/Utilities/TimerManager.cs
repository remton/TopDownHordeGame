using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add a timer component to your script
public class TimerManager : MonoBehaviour
{
    class Timer {
        public Action onEnd;
        public float timeLeft;
        public bool isTimerRunning;
    }

    private static List<Timer> timers;
    public static TimerManager instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        timers = new List<Timer>();
    }

    public static void CreateTimer(float time, Action onEnd) {
        if (instance == null)
            Debug.LogError("No TimerManager Instance!!");

        Timer newTimer = new Timer();
        newTimer.onEnd = onEnd;
        newTimer.timeLeft = time;
        newTimer.isTimerRunning = true;
        timers.Add(newTimer);
    }

    private static void RemoveTimer(int index) {
        timers.RemoveAt(index);
    }

    private void Update(){
        for (int i = 0; i < timers.Count; i++) {
            if (timers[i].isTimerRunning) {
                timers[i].timeLeft -= Time.deltaTime;
                if (timers[i].timeLeft <= 0) {
                    timers[i].onEnd();
                    RemoveTimer(i);
                }
            }
        }
    }

    private void OnDestroy() {
        //for (int i = 0; i <  numTimers; i++) {
        //    actionsOnEnd[i]();
        //    RemoveTimer(i);
        //}
    }
}
