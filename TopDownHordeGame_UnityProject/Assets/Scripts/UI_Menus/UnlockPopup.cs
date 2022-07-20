using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPopup : MonoBehaviour
{
    public float uptime;
    [SerializeField] private Text text;
    [SerializeField] private ImageAnimation anim;
    [SerializeField] private Timer timer;

    public void Activate(string message) {
        gameObject.SetActive(true);
        text.text = message;
        anim.Play();
        timer.CreateTimer(uptime, Hide);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
