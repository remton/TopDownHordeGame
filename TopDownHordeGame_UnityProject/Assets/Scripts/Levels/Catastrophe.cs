using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Timer))]
public class Catastrophe : NetworkBehaviour {
    public static string LevelName = "Catastrophe";

    [SerializeField] private float countdownTime;
    [SerializeField] private List<Prop_Screen> countdownScreens;

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

    public override void OnStartServer() {
        base.OnStartServer();
        countdownID = timer.CreateTimer(countdownTime, CountdownEnd);
    }

    [Server]
    private void ServerUpdate() {
        countdownTimeLeft = timer.TimeLeft(countdownID);
    }
    [Client]
    private void ClientUpdate() {
        foreach (Prop_Screen screen in countdownScreens) {
            screen.SetText(Utilities.FormatTime(countdownTimeLeft));
        }
    }

    [Server]
    private void CountdownEnd() {
        Debug.Log("BOOM!");
    }
}
