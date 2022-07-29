using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(Timer))]
public class MoneyEffect : NetworkBehaviour
{
    public float solidTime;     //Time before this effect starts to fade
    public float fadeTime;      //time that this effect takes to fade
    public float upDist;        //distance to float upwards

    [SerializeField]
    private SpriteRenderer spriteRenderer; 
    [SerializeField]
    private Text text;
    private Timer timer;
    private System.Guid fadeTimerID;
    private bool isFading = false;
    private int amount;

    [Server]
    public void SetAmount(int amount) {
        this.amount = amount;
        text.text = "+ " + amount.ToString();
        SetTextRPC(text.text);
    }

    [Server]
    public void HideAmount() {
        text.text = "";
        SetTextRPC(text.text);
    }
    [ClientRpc]
    private void SetTextRPC(string newText) {
        text.text = newText;
    }

    public int GetAmount() { 
        return amount;
    }

    private void FadeOver() {
        Destroy(gameObject);
    }
    private void FadeUpdate() {
        float ratio = timer.TimeLeft(fadeTimerID) / fadeTime;
        spriteRenderer.color = new Color(1, 1, 1, ratio);
        text.color = new Color(text.color.r, text.color.g, text.color.b, ratio);
    }
    private void StartFade() {
        isFading = true;
        fadeTimerID = timer.CreateTimer(fadeTime, FadeOver);
    }
    private void Update() {
        if (isFading)
            FadeUpdate();
        transform.position += new Vector3(0, upDist*Time.deltaTime/(fadeTime+solidTime), 0);
    }
    private void Start() {
        timer.CreateTimer(solidTime, StartFade);
    }
    private void Awake() {
        timer = GetComponent<Timer>();
    }
}
