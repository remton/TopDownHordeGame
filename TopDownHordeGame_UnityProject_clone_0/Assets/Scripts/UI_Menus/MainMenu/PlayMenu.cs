using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayMenu : Menu
{
    public MainMenuController mainMenu;
    public InputField code;

    public string onlineLobbySceneName;
    //public string offlineLobbySceneName;

    public void OpenMenu() {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }
    public void CloseMenu() {
        gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenu.defaultSelectedObject);
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
