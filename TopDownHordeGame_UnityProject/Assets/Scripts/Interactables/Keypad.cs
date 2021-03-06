using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour {
    private bool isLockedOut = false;

    private HitBoxController hitBox;
    public KeypadUI UI;

    public int[] code;
    public int unlockedDigits;

    public delegate void Guess();
    public event Guess EventCorrectGuess;
    public event Guess EventWrongGuess;

    public void SetUnlockedDigits(int num) {
        unlockedDigits = num;
        List<int> unlockedCode = new List<int>();
        for (int i = 0; i < Mathf.Min(code.Length, unlockedDigits); i++) {
            unlockedCode.Add(code[i]);
        }
        UI.UpdateUnlockedCode(unlockedCode);
    }
    public void SetCode(int[] newCode) {
        code = newCode;
    }
    public void SetLockout(bool b) {
        isLockedOut = b;
        UI.isLockedOut = b;
        if (b) {
            AudioManager.instance.PlaySound(UI.lockoutSound);
            UI.CloseUI();
        }
    }
    public bool IsLockedOut() {
        return isLockedOut;
    }

    private bool CorrectCode(int[] guess) {
        if (guess.Length != code.Length)
            return false;
        for (int i = 0; i < guess.Length; i++) {
            if (guess[i] != code[i])
                return false;
        }
        return true;
    }


    public void OnSubmit(int[] guess) {
        if(CorrectCode(guess)) {
            if (EventCorrectGuess != null) { EventCorrectGuess.Invoke(); }
            UI.CloseUI();
        }
        else {
            if (EventWrongGuess != null) { EventWrongGuess.Invoke(); }
            UI.Clear();
        }
    }


    private void Awake() {
        hitBox = GetComponent<HitBoxController>();
        hitBox.EventObjEnter += OnPlayerEnter;
        hitBox.EventObjExit += OnPlayerExit;
        UI.EventSubmitPressed += OnSubmit;
        SetUnlockedDigits(unlockedDigits);
    }

    private void OnDestroy() {
        hitBox.EventObjEnter -= OnPlayerEnter;
        hitBox.EventObjExit -= OnPlayerExit;
        UI.EventSubmitPressed -= OnSubmit;
    }

    private void OnPlayerEnter(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate += PlayerActivate;
    }
    private void PlayerActivate(GameObject player) {
        UI.OpenUI(player);
    }

    private void OnPlayerExit(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= PlayerActivate;
    }

}
