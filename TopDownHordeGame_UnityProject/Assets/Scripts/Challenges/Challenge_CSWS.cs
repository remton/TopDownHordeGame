using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Timer))]
public class Challenge_CSWS : Challenge
{
    private const float CSWS_UPDATE_TIME = .5f; // Frequency of checks
    private bool roundComplete = false; // Used to check if the player moved this round. Starts false, changes to true each round. Changes to flase when the player stops moving. 
    private Timer timer;
    public override void Load() {
        base.Load();
        timer = GetComponent<Timer>(); // Timer initialized
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

    private void CSWS_Check() // Check if the challenge has been failed 
    {
        Debug.Log("CHECKING CSWS");

        foreach (var player in PlayerManager.instance.GetActiveLocalPlayers())
        {
            //Debug.Log("CSWS foreach");
            //If the player hasn't moved since the last check for CSWS
            if (player.GetComponent<PlayerMovement>().lastMoved > CSWS_UPDATE_TIME)
            {
                Debug.Log("FAILED CSWS");
                roundComplete = false;
            }
            //Debug.Log("CSWS after if");
        }
        if (roundComplete && !completed)
        {
            Debug.Log("CSWS resetting");
            timer.CreateTimer(CSWS_UPDATE_TIME, CSWS_Check);
            Debug.Log("CSWS Timer restarted");
        }
    }
    private void CSWS_RoundCheck(int round)
    {
        if (!completed)
        {
            //if (round == 1)
            //{
            //    roundComplete = true;
            //    Debug.Log("CSWS Line 60");
            //    CSWS_Check();
            //    Debug.Log("First round CSWS check.");
            //}
            //else
            if (round > 1)
            {
                if (roundComplete)
                {
                    Debug.Log("Completing CSWS");
                    Complete();
                }
            }
                roundComplete = true;
                Debug.Log("Not completed. Restarting CSWS.");
                CSWS_Check();
            //}
        }
        else
        {
            RoundController.instance.EventRoundChange -= CSWS_RoundCheck;
        }
    }
    public override void Complete() {
        base.Complete();
        SaveData.instance.modifier_unlocks[(int)ModifierType.CSWSMovement] = true;
        Debug.Log("Completed CSWS");
    }
}
