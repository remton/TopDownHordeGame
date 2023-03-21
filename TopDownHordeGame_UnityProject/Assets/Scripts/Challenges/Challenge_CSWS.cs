using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge_CSWS : Challenge
{
    private const float CSWS_UPDATE_TIME = .5f;
    public bool roundComplete = true;
    private Timer timer;
    public override void Load() {
        base.Load();
        Debug.Log("CSWS Load()");
        Unlock(); // This is unlocked by default
    }

    //Called whenever a level is loaded. Alternative to start function
    protected override void OnLevelWasLoaded(int level) {
        base.OnLevelWasLoaded(level);
        Debug.Log("CSWS OnLevelWasLoaded()");
        SceneLoader.instance.AddPostLoad(StartCheck);
    }

    public void StartCheck() {
        Debug.Log("CSWS StartCheck()");
        if (RoundController.instance != null) {
            RoundController.instance.EventRoundChange += CSWS_RoundCheck;
        }
    }

    private void CSWS_Check()
    {
        Debug.Log("CHECKING Can't Stop Won't Stop");

        foreach (var player in PlayerManager.instance.GetActiveLocalPlayers())
        {
            //If the player hasn't moved since the last check for CSWS
            if (player.GetComponent<PlayerMovement>().lastMoved > CSWS_UPDATE_TIME)
            {
                Debug.Log("FAILED Can't Stop Won't Stop");
                roundComplete = false;
            }
        }
        if(roundComplete && !completed)
            timer.CreateTimer(CSWS_UPDATE_TIME, CSWS_Check);
    }
    private void CSWS_RoundCheck(int round)
    {
        if(round == 1)
        {
            CSWS_Check(); 
            Debug.Log("First round check.");
        }
        else if (round > 1)
        {
            if(roundComplete)
            {
                Complete();
                Debug.Log("Completing Can't Stop Won't Stop");
            }
            roundComplete = true;
            CSWS_Check();
            Debug.Log("Not completed. Restarting.");
        }
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.modifier_unlocks[(int)ModifierType.CSWSMovement] = true;
        Debug.Log("Completed Can't Stop Won't Stop");
    }
}
