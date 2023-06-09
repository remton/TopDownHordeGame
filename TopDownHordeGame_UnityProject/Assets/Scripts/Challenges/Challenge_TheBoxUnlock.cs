using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_TheBoxUnlock : Challenge
{
    public const int roundsToUnlock = 5;
    private int highestRound = 0;
    public override void Load() {
        base.Load();
        Unlock(); // This is unlocked by default
        highestRound = SaveData.instance.leaderboard_highestRound;
        progress = Mathf.Min((highestRound/roundsToUnlock), 1); // The *9 here is due to splitters having 9 total deaths. 1 -> 2 -> 6; 1 + 2 + 6 = 9
        if (progress >= 1)
            Complete();
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.level_unlocks[(int)LevelType.TheBox] = true;
        Debug.Log("Unlocked The box!");
    }
    public override string ProgressText() {
        string txt;
        if (completed)
            txt = "Complete!";
        else
            txt = "Highest Round: " + highestRound.ToString() + " / " + roundsToUnlock.ToString();
        return txt;
    }
}
