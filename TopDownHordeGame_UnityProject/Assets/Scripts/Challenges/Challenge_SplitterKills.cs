using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_SplitterKills : Challenge
{
    public const int challengeKillCount = 10;
    public override void Load() {
        base.Load();
        Unlock(); // This is unlocked by default
        int numKills = SaveData.instance.challenge_splitterKills;
        progress = Mathf.Min((float)numKills / (challengeKillCount * 9), 1);
        if (progress >= 1)
            Complete();
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.modifier_unlocks[(int)ModifierType.allSplitter] = true;
        Debug.Log("Completed Splitter Kills");
    }
}
