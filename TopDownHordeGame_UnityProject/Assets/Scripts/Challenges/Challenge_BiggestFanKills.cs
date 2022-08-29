using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_BiggestFanKills : Challenge
{
    public const int challengeKillCount = 10;
    public override void Load() {
        base.Load();
        Unlock(); // This is unlocked by default
        int numKills = SaveData.instance.challenge_biggestFanKills;
        progress = Mathf.Min((float)numKills / challengeKillCount, 1);
        if (progress >= 1)
            Complete();
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.modifier_unlocks[(int)ModifierType.allBiggestFan] = true;
        Debug.Log("Completed Biggest Fan Kills");
    }
}
