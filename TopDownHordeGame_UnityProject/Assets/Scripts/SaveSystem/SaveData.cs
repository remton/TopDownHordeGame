//This class holds the data that may be saved in game
//The data is loaded at runtime

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    // ------ Data ------
    [Header("Data loaded at Runtime")]
    [Header("Settings")]
    public float settings_volumeSFX;
    public float settings_volumeMusic;
    [Header("CatCafe")]
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;


    // ---- Instance handling ----
    public static SaveData instance;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        Load();
    }


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
        instance.settings_volumeSFX = save.settings_volumeSFX;
        instance.settings_volumeMusic = save.settings_volumeMusic;
        //Cat Cafe
        instance.catCafe_code = save.catCafe_code;
        instance.catCafe_unlockedDigits = save.catCafe_unlockedDigits;
    }
    public static void DeleteSave() {
        SaveSystem.DeleteSave();
    }
    public static void ClearData() {
        //settings
        instance.settings_volumeSFX = 0.5f;
        instance.settings_volumeMusic = 0.5f;
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
