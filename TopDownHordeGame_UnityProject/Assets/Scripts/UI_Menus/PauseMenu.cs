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
        if (PauseManager.instance.IsPaused())
            PauseManager.instance.PauseButtonPress();
    }
    public void Button_LeaveGame() {
        if (PauseManager.instance.IsPaused())
            PauseManager.instance.UnpauseTime();
        QuitToMainMenu();
    }
    public override void OnCancel() {
        base.OnCancel();
        PauseManager.instance.PauseButtonPress();
    }

    public void QuitToMainMenu() {
        if (PlayerConnection.myConnection.isServer)
            SaveData.Save();
        PlayerConnection.myConnection.Disconnect();
    }

    public override void Close() {
        base.Close();
        settings.Close();
    }
}
