using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a timer component to your script and make as many timers as you want :D
public class Timer : MonoBehaviour
{
    class SingleTimer {
        public Action onEnd;
        public float timeLeft;
        public bool isTimerRunning;
    }

    private List<SingleTimer> timers;

    private void Awake() {
        timers = new List<SingleTimer>();
    }

    public void CreateTimer(float time, Action onEnd) {

        SingleTimer newTimer = new SingleTimer();
        newTimer.onEnd = onEnd;
        newTimer.timeLeft = time;
        newTimer.isTimerRunning = true;
        timers.Add(newTimer);
    }

    private void RemoveTimer(int index) {
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
