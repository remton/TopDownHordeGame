using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    const string STEAM_TEXT = "Enter a game code";
    const string NO_STEAM_TEXT = "Please start Steam for online play";

    public InputField code;
    public Button hostButton;
    public Button joinButton;
    public Text connectedTxt;

    //Scene to load when hosting or joining offline
    [Mirror.Scene]
    public string lobbyScene;

    //Used to allow KCP transport for testing networking on a single machine bia parrelSync
    [SerializeField]
    private bool kcpEnabled;

    public override void Open() {
        base.Open();
        ConnectToSteam(MyNetworkManager.instance.useSteam);
    }

    public void ConnectToSteam(bool connected) {
        if (connected)
            EnableOnline();
        else
            DisableOnline();
    }
    public void EnableOnline() {
        hostButton.interactable = true;
        joinButton.interactable = true;
        code.interactable = true;
        connectedTxt.text = STEAM_TEXT;
    }
    public void DisableOnline() {
        if (!kcpEnabled) {
            hostButton.interactable = false;
            joinButton.interactable = false;
        }
        code.interactable = false;
        connectedTxt.text = NO_STEAM_TEXT;
    }
    public void FailedToJoin(string info) {
        connectedTxt.text = info;
    }

    //Buttons in this menu
    public void Button_Host() {
        MyNetworkManager.instance.HostGame(code.text);
    }

    public void Button_Join() {
        MyNetworkManager.instance.JoinGame(code.text);
    }

    public void Button_Offline() {
        MyNetworkManager.instance.HostOffline();
    }

    public void Button_Close() {
        Close();
    }

    private void Start() {
        MyNetworkManager.instance.onlineScene = lobbyScene;
        MyNetworkManager.instance.Event_FailedToCreateLobby += FailedToJoin;
        MyNetworkManager.instance.Event_FailedToJoinLobby += FailedToJoin;
    }
    private void OnDestroy() {
        MyNetworkManager.instance.Event_FailedToCreateLobby -= FailedToJoin;
        MyNetworkManager.instance.Event_FailedToJoinLobby -= FailedToJoin;
    }
}
