//This class holds the data that may be saved in game
//The data is loaded at runtime

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour {
    // -------- Data --------
    [Header("Data loaded at Runtime")]
    [Header("Settings")]

    private float internal_settings_volumeMaster;
    public float settings_volumeMaster {
        get { return internal_settings_volumeMaster; }
        set { internal_settings_volumeMaster = value; if (EventSettingsChange != null) { EventSettingsChange.Invoke(this); } }
    }

    private float internal_settings_volumeSFX;
    public float settings_volumeSFX {
        get { return internal_settings_volumeSFX; }
        set { internal_settings_volumeSFX = value; if (EventSettingsChange != null) { EventSettingsChange.Invoke(this); } }
    }

    private float internal_settings_volumeMusic;
    public float settings_volumeMusic {
        get { return internal_settings_volumeMusic; }
        set { internal_settings_volumeMusic = value; if (EventSettingsChange != null) { EventSettingsChange.Invoke(this); } }
    }

    [Header("Leaderboard")]
    public int leaderboard_totalKills;
    public int leaderboard_mostKills;
    public int leaderboard_totalMoneyEarned;
    public int leaderboard_mostMoneyEarned;
    public int leaderboard_totalScore;
    public int leaderboard_mostScore;
    public int leaderboard_highestRound;

    [Header("CatCafe")]
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;


    // ---- Instance handling ----
    public static SaveData instance;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        Load();
    }

    // ---- On Change Events ----
    public delegate void SettingsChange(SaveData data);
    public event SettingsChange EventSettingsChange;


    // ----- Public Methods -----
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

        // Copy all data to instance
        //Settings
        instance.settings_volumeMaster = save.settings_volumeMaster;
        instance.settings_volumeSFX = save.settings_volumeSFX;
        instance.settings_volumeMusic = save.settings_volumeMusic;
        //Leaderboard
        instance.leaderboard_highestRound = save.leaderboard_highestRound;
        instance.leaderboard_mostKills = save.leaderboard_mostKills;
        instance.leaderboard_mostMoneyEarned = save.leaderboard_mostMoneyEarned;
        instance.leaderboard_mostScore = save.leaderboard_mostScore;
        instance.leaderboard_totalKills = save.leaderboard_totalKills;
        instance.leaderboard_totalMoneyEarned = save.leaderboard_totalMoneyEarned;
        instance.leaderboard_totalScore = save.leaderboard_totalScore;
        //Cat Cafe
        instance.catCafe_code = save.catCafe_code;
        instance.catCafe_unlockedDigits = save.catCafe_unlockedDigits;
    }
    public static void DeleteSave() {
        SaveSystem.DeleteSave();
    }
    public static void ClearData() {
        //settings
        instance.settings_volumeMaster = 0.5f;
        instance.settings_volumeSFX = 0.5f;
        instance.settings_volumeMusic = 0.5f;
        //Leaderboard
        instance.leaderboard_highestRound = 0;
        instance.leaderboard_mostKills = 0;
        instance.leaderboard_mostMoneyEarned = 0;
        instance.leaderboard_mostScore = 0;
        instance.leaderboard_totalKills = 0;
        instance.leaderboard_totalMoneyEarned = 0;
        instance.leaderboard_totalScore = 0;
        //CatCafe
        instance.catCafe_code = new int[CatCafe.codeLength];
        for (int i = 0; i < CatCafe.codeLength; i++) {
            instance.catCafe_code[i] = Random.Range(0, 10);
        }
        instance.catCafe_unlockedDigits = 0;
        instance.catCafe_unlockedElevator = false;
    }



    // ---- Editor tools ----
    [Header("Editor Buttons")]
    public bool saveButton;
    public bool loadButton;
    public bool deleteSaveButton;
    public bool clearDataButton;
    private void Update() {
        if (saveButton) { Save(); saveButton = false; }
        if (loadButton) { Load(); loadButton = false; }
        if (deleteSaveButton) { DeleteSave(); deleteSaveButton = false; }
        if (clearDataButton) { ClearData(); clearDataButton = false; }
    }
}
