using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Catastrophe : NetworkBehaviour {
    public static string LevelName = "Catastrophe";

    [SerializeField] private float countdownTime;
    [SerializeField] private List<Prop_Screen> countdownScreens;
    public AudioClip timerBeepSound;
    public float timerBeepVolume;

    private Timer timer;
    private System.Guid countdownID;
    [SyncVar] private float countdownTimeLeft;
    private void Awake() {
        timer = GetComponent<Timer>();
    }
    private void Update() {
        if(isServer)
            ServerUpdate();

        ClientUpdate();
    }

    public IEnumerator TimerBeep() {
        while (countdownTimeLeft > 0) {
            yield return new WaitForSeconds(1);
            AudioManager.instance.PlaySound(timerBeepSound, timerBeepVolume);
        }
    }

    public override void OnStartServer() {
        base.OnStartServer();
        countdownID = timer.CreateTimer(countdownTime, CountdownEnd);
    }

    public override void OnStartClient() {
        base.OnStartClient();
        StartCoroutine(TimerBeep());
    }

    [Server]
    private void ServerUpdate() {
        countdownTimeLeft = timer.TimeLeft(countdownID);
    }
    [Client]
    private void ClientUpdate() {
        foreach (Prop_Screen screen in countdownScreens) {
            string txt = "DETENATION IN \n" + Utilities.FormatTime(countdownTimeLeft);
            screen.SetText(txt);
        }
    }

    [Server]
    private void CountdownEnd() {
        Debug.Log("BOOM!");
    }
}
