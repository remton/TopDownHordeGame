/* Timer.cs
 * Author: Remington Ward
 * Date: 1/24/2022
 * This script is built for the Unity Game Engine. It provides a simple way to wait an amount of time for any script.
 * Just add the Timer script to the object requiring some wait time, the with a reference to it ie. Timer timer
 * call timer.CreateTimer(float time, Action methodOnTimerEnd);
 * The method for the timer's end must have no arguments and return void
 * if you need to control the timer include "using system" and use Guid return from CreateTimer as the parameter to the control methods
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a timer component to your script and make as many timers as you want :D
public class Timer : MonoBehaviour
{
    public Guid CreateTimer(float time, Action onEnd) {
        SingleTimer newTimer = new SingleTimer();
        newTimer.ID = Guid.NewGuid();
        newTimer.onEnd = onEnd;
        newTimer.timeLeft = time;
        newTimer.isTimerRunning = true;
        timers.Add(newTimer);
        return newTimer.ID;
    }
    public void PauseTimer(Guid timerID) {
        foreach (SingleTimer timer in timers) {
            if(timer.ID == timerID) {
                timer.isTimerRunning = false;
            }
        }
    }
    public void UnPauseTimer(Guid timerID) {
        foreach (SingleTimer timer in timers) {
            if (timer.ID == timerID) {
                timer.isTimerRunning = false;
            }
        }
    }
    public void KillTimer(Guid timerID) {
        for (int i = 0; i < timers.Count; i++) {
            if (timers[i].ID == timerID) {
                RemoveTimer(i);
            }
        }
    }

    private class SingleTimer {
        public Guid ID;
        public Action onEnd;
        public float timeLeft;
        public bool isTimerRunning;
    }
    private List<SingleTimer> timers = new List<SingleTimer>();
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
}
