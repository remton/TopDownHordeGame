using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicTimer : MonoBehaviour
{
    public Image Image;
    public Text text;
    private bool timerRunning = false;
    private Magic magic;
    private Timer timer;
    private System.Guid timerID;

    public void StartTimer(Magic magic, System.Guid timerID) {
        this.magic = magic;
        this.timerID = timerID;
        timer = magic.GetComponent<Timer>();
        magic.EventTimerEnd += RemoveTimer;
        timerRunning = true;
        Image.sprite = magic.icon;
    }
    public void RemoveTimer() {
        magic.EventTimerEnd -= RemoveTimer;
        MagicController.instance.RemoveTimer(this);
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
