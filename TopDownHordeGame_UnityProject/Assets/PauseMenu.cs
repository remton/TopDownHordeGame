using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : Menu {
    public void Button_Continue() {
        if (PauseManager.instance.IsPaused())
            PauseManager.instance.PauseButtonPress();
    }
    public void Button_LeaveGame() {
        CloseMenu();
        QuitToMainMenu();
    }

    public void OpenMenu() {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }
    public void CloseMenu() {
        gameObject.SetActive(false);
    }
    public void QuitToMainMenu() {
        if (PlayerConnection.myConnection.isServer)
            SaveData.Save();
        PlayerConnection.myConnection.Disconnect();
    }
}
