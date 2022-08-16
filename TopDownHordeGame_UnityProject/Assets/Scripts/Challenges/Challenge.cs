using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
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

    public abstract void Load();
    
    public abstract void Unlock();

    public abstract void Complete();

    private void Start() {
        Load();
    }
    private void Awake() {
        allChallenges.Add(this);
    }
}
