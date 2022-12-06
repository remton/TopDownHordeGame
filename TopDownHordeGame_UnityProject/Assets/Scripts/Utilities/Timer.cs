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
    private List<SingleTimer> timers = new List<SingleTimer>();
    private List<bool> pauseState = new List<bool>();

    public Guid CreateTimer(float time, Action onEnd) {
        SingleTimer newTimer = new SingleTimer();
        newTimer.ID = Guid.NewGuid();
        newTimer.onEnd = onEnd;
        newTimer.timeLeft = time;
        newTimer.isTimerRunning = true;
        timers.Add(newTimer);
        pauseState.Add(true);
        return newTimer.ID;
    }
    public void SetTimer(Guid timerID, float newTime, Action newOnEnd) {
        foreach (SingleTimer timer in timers) {
            if (timer.ID == timerID) {
                timer.timeLeft = newTime;
                timer.onEnd = newOnEnd;
                return;
            }
        }
    }
    public void PauseTimer(Guid timerID) {
        foreach (SingleTimer timer in timers) {
            if(timer.ID == timerID) {
                timer.isTimerRunning = false;
            }
        }
    }
    public void UnpauseTimer(Guid timerID) {
        foreach (SingleTimer timer in timers) {
            if (timer.ID == timerID) {
                timer.isTimerRunning = false;
            }
        }
    }
    public float TimeLeft(Guid timerID) {
        foreach (SingleTimer timer in timers) {
            if (timer.ID == timerID) {
                return timer.timeLeft;
            }
        }
        return 0;
    }
    public bool HasTimer(Guid timerID) {
        if (timerID == Guid.Empty)
            return false;

        foreach (SingleTimer timer in timers) {
            if (timer.ID == timerID) {
                return true;
            }
        }
        return false;
    }

    /// <summary> Saves which timers are running in th current moment into a local pause state </summary>
    public void SavePauseState() {
        pauseState.Clear();
        foreach (SingleTimer timer in timers) {
            pauseState.Add(timer.isTimerRunning);
        }
    }
    public void LoadPauseState() {
        for (int i = 0; i < timers.Count; i++) {
            timers[i].isTimerRunning = pauseState[i];
        }
    }

    public void PauseAll() {
        foreach (SingleTimer timer in timers) {
            timer.isTimerRunning = false;
        }
    }
    public void UnPauseAll() {
        foreach (SingleTimer timer in timers) {
            timer.isTimerRunning = true;
        }
    }

    public void KillTimer(Guid timerID) {
        for (int i = 0; i < timers.Count; i++) {
            if (timers[i].ID == timerID) {
                RemoveTimer(i);
            }
        }
    }

    [Serializable]
    private class SingleTimer {
        public Guid ID;
        public Action onEnd;
        public float timeLeft;
        public bool isTimerRunning;
    }
    private void RemoveTimer(int index) {
        if(index >= timers.Count) {
            Debug.LogError("ERROR: Timer index out of range");
        }
        timers.RemoveAt(index);
        pauseState.RemoveAt(index);
    }
    private void Update(){
        for (int i = 0; i < timers.Count; i++) {
            if (timers[i].isTimerRunning) {
                timers[i].timeLeft -= Time.deltaTime;
                //Timer is done
                if (timers[i].timeLeft <= 0) {
                    //Call onEnd()
                    Guid id = timers[i].ID;
                    if(timers[i].onEnd != null)
                        timers[i].onEnd();
                    //Ensure timers was not modified during onEnd()
                    if (i >= timers.Count || id != timers[i].ID) {
                        Debug.LogWarning("WARNING: Modifying timers during a timer's return function");
                        KillTimer(id);
                        break;
                    }
                    RemoveTimer(i);
                    i--;
                }
            }
        }
    }
}
