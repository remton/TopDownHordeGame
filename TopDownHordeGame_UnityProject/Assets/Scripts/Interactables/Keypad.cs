using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : Interactable {
    private bool isLockedOut = false;

    //private HitBoxController hitBox;
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

        UI.EventSubmitPressed += OnSubmit;
        SetUnlockedDigits(unlockedDigits);
    }

    private void OnDestroy() {
        UI.EventSubmitPressed -= OnSubmit;
    }


    public override void OnPlayerEnter(GameObject player) {
        base.OnPlayerEnter(player);
    }
    public override void OnInteract(GameObject player) {
        base.OnInteract(player);
        UI.OpenUI(player);
    }
    public override void OnPlayerExit(GameObject player) {
        base.OnPlayerExit(player);
    }

}
