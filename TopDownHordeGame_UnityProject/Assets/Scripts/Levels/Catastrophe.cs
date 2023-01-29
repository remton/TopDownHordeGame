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

    [SerializeField] private List<GameObject> powerDependents;
    [SerializeField] private GameObject darkness;

    [SerializeField] private Interactable powerSwitch;

    private Timer timer;
    private System.Guid countdownID;
    private float countdownTimeLeft;
    private int prevCountdownTimeLeft;
    public bool isPowerOn { get; internal set; }

    private void Awake() {
        timer = GetComponent<Timer>();
        countdownTimeLeft = countdownTime;
    }
    private void Update() {
        ClientUpdate();
    }
    [Client]
    private void ClientUpdate() {
        countdownTimeLeft = timer.TimeLeft(countdownID);
        foreach (Prop_Screen screen in countdownScreens) {
            string txt = "DETENATION IN \n" + Utilities.FormatTime(countdownTimeLeft);
            if (isPowerOn)
                screen.SetText(txt);
            else
                screen.SetText("");
        }
        if(Mathf.RoundToInt(countdownTimeLeft) < prevCountdownTimeLeft) {
            AudioManager.instance.PlaySound(timerBeepSound, timerBeepVolume);
        }
        prevCountdownTimeLeft = Mathf.RoundToInt(countdownTimeLeft);
    }

    public override void OnStartServer() {
        base.OnStartServer();
        TurnOffPower();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        powerSwitch.EventOnInteract += PowerSwitch;
    }

    [Client]
    private void StartCountdown() {
        if (!timer.HasTimer(countdownID)) {
            countdownID = timer.CreateTimer(countdownTime, CountdownEnd);
            Debug.Log("Starting COUNTDOWN");
        }
        else {
            Debug.Log("unpause COUNTDOWN");
            timer.UnpauseTimer(countdownID);
        }
    }
    [Client]
    private void PauseCountdown() {
        Debug.Log("pause COUNTDOWN");
        timer.PauseTimer(countdownID);
    }
    [Client]
    private void CountdownEnd() {
        Debug.Log("BOOM!");
    }



    [Command(requiresAuthority = false)]
    public void PowerSwitch(GameObject player) {
        Debug.Log("POWER SWITCH");
        if (isPowerOn)
            TurnOffPower();
        else
            TurnOnPower();
    } 

    [Server]
    private void TurnOffPower() {
        darkness.SetActive(true);
        isPowerOn = false;
        foreach (GameObject item in powerDependents) {
            if (!item.HasComponent<Interactable>())
                continue;
            item.GetComponent<Interactable>().SetInteractable(false);
        }
        TurnOffPowerRPC();
    }
    [ClientRpc]
    private void TurnOffPowerRPC() {
        darkness.SetActive(true);
        isPowerOn = false;
        PauseCountdown();
    }

    [Server]
    private void TurnOnPower() {
        darkness.SetActive(false);
        isPowerOn = true;
        foreach (GameObject item in powerDependents) {
            if (!item.HasComponent<Interactable>())
                continue;
            item.GetComponent<Interactable>().SetInteractable(true);
        }
        TurnOnPowerRPC();
    }
    [ClientRpc]
    private void TurnOnPowerRPC() {
        darkness.SetActive(false);
        isPowerOn = true;
        StartCountdown();
    }
}
