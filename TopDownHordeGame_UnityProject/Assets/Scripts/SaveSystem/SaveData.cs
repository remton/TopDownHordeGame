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
    public bool clearDataButton;
    private void Update() {
        if (saveButton) { Save(); saveButton = false;}
        if (loadButton) { Load(); loadButton = false;}
        if (clearDataButton) { ClearData(); clearDataButton = false;}
    }

    // ------ Data ------
    [Header("Data loaded at Runtime")]
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
        instance.catCafe_code = save.catCafe_code;
        instance.catCafe_unlockedDigits = save.catCafe_unlockedDigits;
    }

    public static void ClearData() {
        //Generate new code for CatCafe
        instance.catCafe_code = new int[4];
        for (int i = 0; i < 4; i++) {
            instance.catCafe_code[i] = Random.Range(0, 10);
        }
        instance.catCafe_unlockedDigits = 0;
        instance.catCafe_unlockedElevator = false;
    }
}
