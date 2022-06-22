using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    const string STEAM_TEXT = "Connected to Steam";
    const string NO_STEAM_TEXT = "Please start Steam for online play";

    public MainMenuController mainMenu;
    public InputField code;
    public Button hostButton;
    public Button joinButton;
    public Text connectedTxt;

    public string onlineLobbySceneName;
    //public string offlineLobbySceneName;

    [SerializeField]
    private bool kcpEnabled;

    public override void Open() {
        mainMenu.Close();
        base.Open();
        ConnectedToSteam(MyNetworkManager.instance.useSteam);
    }

    public void CloseMenu() {
        base.Close();
        mainMenu.Open();
    }

    public void ConnectedToSteam(bool connected) {
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
        CloseMenu();
    }

    private void Start() {
        MyNetworkManager.instance.onlineScene = onlineLobbySceneName;
    }
}
