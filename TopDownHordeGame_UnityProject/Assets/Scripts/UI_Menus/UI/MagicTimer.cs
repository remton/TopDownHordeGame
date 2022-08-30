using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicTimer : MonoBehaviour
{
    public Image Image;
    public Text text;
    private Magic magic;
    private System.Guid timerID;
    private bool started = false;

    public void StartTimer(Magic magic, System.Guid timerID) {
        this.magic = magic;
        this.timerID = timerID;
        magic.EventTimerEnd += RemoveTimer;
        Image.sprite = magic.icon;
        started = true;
    }
    public void RemoveTimer() {
        magic.EventTimerEnd -= RemoveTimer;
        MagicController.instance.RemoveTimer(this);
    }
    private void Update() {
        if (!started)
            return;

        if(magic != null && magic.GetComponent<Timer>().HasTimer(timerID)) {
            int timeLeft = Mathf.FloorToInt(magic.GetComponent<Timer>().TimeLeft(timerID));
            int min = timeLeft / 60;
            int sec = timeLeft % 60;
            string newText = string.Format("{0:0}:{1:00}", min, sec);
            text.text = newText;
        }
        else {
            RemoveTimer();
        }
    }
}
