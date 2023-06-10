using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_CatastropheUnlock : Challenge
{
    public override void Load() {
        base.Load();
        Unlock(); // This is unlocked by default
        if (SaveData.instance.catCafe_unlockedElevator)
            progress = 1;
        else
            progress = 0;

        if (progress >= 1)
            Complete();
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.level_unlocks[(int)LevelType.Catastrophe] = true;
        Debug.Log("Unlocked Catastrophe!");
    }
    public override string ProgressText() {
        string txt;
        if (completed)
            txt = "Complete!";
        else
            txt = "Incomplete";
        return txt;
    }
}
