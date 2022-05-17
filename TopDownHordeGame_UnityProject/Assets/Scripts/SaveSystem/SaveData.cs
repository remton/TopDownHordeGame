//This class holds the data that may be saved in game
//The data is loaded at runtime

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    // ---- Editor tools ----
    [Header("Editor Buttons")]
    public bool saveButton;
    public bool loadButton;
    public bool logDataButton;
    private void Update() {
        if (saveButton) { Save(); saveButton = false;}
        if (loadButton) { Load(); loadButton = false;}
        if (logDataButton) { LogData(); logDataButton = false;}
    }

    // ------ Data ------
    [Header("Data loaded at Runtime")]
    public string test;


    // ---- Instance handling ----
    public static SaveData instance;
    private void Awake() {
        if (instance == null) {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            Load();
        }
    }

    // ----- Public Methods -----
    public static void LogData() {
        Debug.Log("test: " + instance.test);
    }

    public static void Save() {
        Debug.Log("Saving . . .");
        SaveSystem.Save(instance);
    }

    public static void Load() {
        Save save = SaveSystem.LoadSave();
        if (save == null) {
            ClearData();
            Save();
            return;
        }

        // Copy all data to this
        instance.test = save.testStr;
    }

    public static void ClearData() {
        instance.test = "NO STRING";
    }
}
