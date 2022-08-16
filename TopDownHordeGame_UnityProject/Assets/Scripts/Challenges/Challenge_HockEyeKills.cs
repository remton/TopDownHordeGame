using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_HockEyeKills : Challenge
{
    public const int challengeKillCount = 10;
    public override void Load() {
        Unlock(); // This is unlocked by default
        int numKills = SaveData.instance.challenge_hockEyeKills;
        progress = Mathf.Min((float)numKills / challengeKillCount, 1);
        if (progress >= 1)
            Complete();
    }
    public override void Complete() {
        completed = true;
        Debug.Log("Completed Biggest Fan Kills");
    }
    public override void Unlock() {
        unlocked = true;
    }
}
