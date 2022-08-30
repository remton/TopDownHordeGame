using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeypadUI : Menu
{
    public bool isLockedOut;
    public AudioClip lockoutSound;
    public AudioClip buttonSound;
    private int numDisplaySize;
    public Text UINumTxt;
    public List<GameObject> digitHolders;

    public delegate void submitPressed(int[] guess);
    public event submitPressed EventSubmitPressed;
    public delegate void cancelPressed();
    public event cancelPressed EventCancelPressed;

    public List<int> currGuess = new List<int>();

    private GameObject interactingPlayer;

    public override void OnCancel() {
        base.OnCancel();
        if (EventCancelPressed != null) { EventCancelPressed.Invoke(); }
        CloseUI();
    }

    public void Clear() {
        currGuess.Clear();
        UpdateUI(currGuess);
    }


    public void PressNum(int num) {
        AudioManager.instance.PlaySound(buttonSound);
        currGuess.Add(num);
        UpdateUI(currGuess);
    }
    public void PressSubmit() {
        AudioManager.instance.PlaySound(buttonSound);
        if (EventSubmitPressed != null) { EventSubmitPressed.Invoke(currGuess.ToArray()); }
    }
    public void PressClear() {
        AudioManager.instance.PlaySound(buttonSound);
        currGuess.Clear();
        UpdateUI(currGuess);
    }

    private void Start() {
        numDisplaySize = UINumTxt.text.Length - 1;
        UpdateUI(currGuess);
    }
    public void SetInteractingPlayer(GameObject player) {
        //Simply remove the interacting player if null param
        if(player == null) {
            if (interactingPlayer != null) {
                //Remove interacting player and reenable them
                interactingPlayer.GetComponent<Player>().EnablePlayer();
                interactingPlayer.GetComponent<PlayerPauseControl>().EnablePause();
            }
            interactingPlayer = null;
            return;
        }
        //remove old player and set new
        Debug.Log(player.name + " is interacting with keypad");
        if (interactingPlayer != null) {
            //Remove interacting player and reenable them
            interactingPlayer.GetComponent<Player>().EnablePlayer();
            interactingPlayer.GetComponent<PlayerPauseControl>().EnablePause();
        }
        //Set new interacting player and disable them
        interactingPlayer = player;
        interactingPlayer.GetComponent<Player>().DisablePlayer();
        interactingPlayer.GetComponent<PlayerPauseControl>().DisablePause();
    }

    public override void Open() {
        Debug.LogError("Do not use Open for keypad UI, use OpenUI instead");
    }
    public override void Close() {
        CloseUI();
    }

    public void OpenUI(GameObject player) {
        if (isLockedOut) {
            AudioManager.instance.PlaySound(lockoutSound);
            return;
        }
        SetInteractingPlayer(player);
        gameObject.SetActive(true);
        Cursor.visible = true;
    }
    public void CloseUI() {
        SetInteractingPlayer(null);
        gameObject.SetActive(false);
        Cursor.visible = false;
    }
    public void UpdateUI(List<int> numPressed) {
        string newText = "";
        foreach (int num in numPressed) {
            newText += num.ToString();
        }
        for (int i = numPressed.Count; i < numDisplaySize; i++) {
            newText += " ";
        }
        UINumTxt.text = newText;
    }
    public void UpdateUnlockedCode(List<int> unlockedCode) {
        for (int i = 0; i < digitHolders.Count; i++) {
            if(i < unlockedCode.Count) {
                digitHolders[i].SetActive(true);
                digitHolders[i].GetComponentInChildren<Text>().text = unlockedCode[i].ToString();
            }
            else {
                digitHolders[i].SetActive(false);
            }
        }
    }
}
