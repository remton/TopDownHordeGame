using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : Menu
{
    public void CloseMenu() {
        gameObject.SetActive(false);
    }
    public void ResetSaveData() {
        SaveData.DeleteSave();
        SaveData.ClearData();
        SaveData.Save();
    }
}
