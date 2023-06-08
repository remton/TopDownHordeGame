using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : Menu {
    public SettingsController settings;
    public void Button_Settings() {
        settings.Open();
    }

    public void Button_Continue() {
        OnCancel();
    }
    public void Button_LeaveGame() {
        if (PauseManager.instance.IsPaused())
            PauseManager.instance.UnpauseTime();
        QuitToMainMenu();
    }
    public override void OnCancel() {
        base.OnCancel();
        if (PauseManager.instance.IsPaused())
            PauseManager.instance.PauseButtonPress();
    }

    public void QuitToMainMenu() {
        if (PlayerConnection.myConnection.isServer) {
            PlayerManager.instance.EndGame(true);
        }
        else {
            PlayerManager.instance.SetGameOverData();
            PlayerManager.instance.SavePlayerData();
            SaveData.Save();
            PlayerConnection.myConnection.Disconnect();
        }
    }

    public override void Close() {
        settings.Close();
        base.Close();
    }
}
