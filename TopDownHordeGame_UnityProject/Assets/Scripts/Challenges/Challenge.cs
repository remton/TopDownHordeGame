using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChallengeType {
    BasicKills, BiggestFanKills, HockEyeKills, LungsKills, SplitterKills, ZathrakKills
}

public class Challenge : MonoBehaviour
{
    public ChallengeType type;
    public string title;
    [TextArea]
    public string description;
    public Sprite icon;
    public float progress;      //progress 0 - 0.1 of completing this challenge
    public bool unlocked;       //if this challenge is available
    public bool completed;      //if this challenge has been completed

    private static List<Challenge> allChallenges = new List<Challenge>();

    public static void ReloadAll() {
        foreach (var item in allChallenges) {
            item.Load();
        }
    }

    public virtual void Load() {
        unlocked = SaveData.instance.challenge_unlocks[(int)type];
        completed = SaveData.instance.challenge_completed[(int)type];
    }
    
    public virtual void Unlock() {
        unlocked = true;
        SaveData.instance.challenge_unlocks[(int)type] = true;
    }

    public virtual void Complete() {
        completed = true;
        SaveData.instance.challenge_completed[(int)type] = true;
    }

    private void Start() {
        Load();
    }
    private void Awake() {
        allChallenges.Add(this);
    }
}
