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

    [Header("Challenges")]
    public bool[] challenge_unlocks;
    public bool[] challenge_completed;
    public int challenge_basicKills;
    public int challenge_biggestFanKills;
    public int challenge_hockEyeKills;
    public int challenge_LungsKills;
    public int challenge_splitterKills;
    public int challenge_zathrakKills;

    [Header("Modifers")]
    public bool[] modifier_unlocks;

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
        try {
            SaveSystem.Save(instance);
        }
        catch (System.IO.IOException e) {
            Debug.LogWarning("FAILED TO SAVE DUE TO IOException." + e.ToString());
        }
    }
    public static void Load() {
        Save save = SaveSystem.LoadSave();
        if (save == null || !ValidSave(save)) {
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
        //Challenges
        instance.challenge_unlocks = save.challenge_unlocks;
        instance.challenge_completed = save.challenge_completed;
        instance.challenge_basicKills = save.challenge_basicKills;
        instance.challenge_biggestFanKills = save.challenge_biggestFanKills;
        instance.challenge_hockEyeKills = save.challenge_hockEyeKills;
        instance.challenge_LungsKills = save.challenge_LungsKills;
        instance.challenge_splitterKills = save.challenge_splitterKills;
        instance.challenge_zathrakKills = save.challenge_zathrakKills;
        //Modifiers
        instance.modifier_unlocks = save.modifier_unlocks;
        //Cat Cafe
        instance.catCafe_code = save.catCafe_code;
        instance.catCafe_unlockedDigits = save.catCafe_unlockedDigits;
    }
    public static bool ValidSave(Save save) {
        if (save.modifier_unlocks == null)
            return false;
        if (save.challenge_unlocks == null)
            return false;
        if (save.challenge_completed == null)
            return false;
        if (save.modifier_unlocks.Length != System.Enum.GetNames(typeof(ModifierType)).Length)
            return false;
        if (save.challenge_unlocks.Length != System.Enum.GetNames(typeof(ChallengeType)).Length)
            return false;
        if (save.challenge_completed.Length != System.Enum.GetNames(typeof(ChallengeType)).Length)
            return false;
        return true;
    }
    public static void DeleteSave() {
        SaveSystem.DeleteSave();
    }
    public static void ClearData() {
        Debug.Log("Clearing save data...");

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
        //Challenges
        instance.challenge_unlocks = new bool[System.Enum.GetNames(typeof(ChallengeType)).Length];
        instance.challenge_completed = new bool[System.Enum.GetNames(typeof(ChallengeType)).Length];
        instance.challenge_basicKills = 0;
        instance.challenge_biggestFanKills = 0;
        instance.challenge_hockEyeKills = 0;
        instance.challenge_LungsKills = 0;
        instance.challenge_splitterKills = 0;
        instance.challenge_zathrakKills = 0;
        //Modifiers
        instance.modifier_unlocks = new bool[System.Enum.GetNames(typeof(ModifierType)).Length];
        //CatCafe
        instance.catCafe_code = new int[CatCafe.codeLength];
        for (int i = 0; i < CatCafe.codeLength; i++) {
            instance.catCafe_code[i] = Random.Range(0, 10);
        }
        instance.catCafe_unlockedDigits = 0;
        instance.catCafe_unlockedElevator = false;

        //Update other scripts
        Challenge.ReloadAll();
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
