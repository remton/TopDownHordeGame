using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicTimer : MonoBehaviour
{
    public Image Image;
    public Text text;

    public delegate void OnTimerEnd(MagicTimer timer);
    public event OnTimerEnd EventOnTimerEnd;

    [SerializeField] private Timer timer;
    private bool timerRunning = false;
    private System.Guid timerID = System.Guid.Empty;
    private float pausedAtTime;
    public void StartTimer(float time, Magic magic) {
        timerID = timer.CreateTimer(time, EndTimer);
        timerRunning = true;
        Image.sprite = magic.icon;
    }
    public void PauseTimer() {
        timer.PauseTimer(timerID);
        timerRunning = false;
    }
    public void UnpauseTimer() {
        timer.UnpauseTimer(timerID);
        timerRunning = true;
    }
    public void EndTimer() {
        timerRunning = false;
        if (EventOnTimerEnd != null) { EventOnTimerEnd.Invoke(this); }
    }

    private void Update() {
        if (timerRunning) {
            int timeLeft = Mathf.FloorToInt(timer.TimeLeft(timerID));
            int min = timeLeft / 60;
            int sec = timeLeft % 60;
            string newText = string.Format("{0:0}:{1:00}", min, sec);
            text.text = newText;
        }
    }
}
