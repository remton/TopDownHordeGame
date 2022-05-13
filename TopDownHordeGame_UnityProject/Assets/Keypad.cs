using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour {

    private HitBoxController hitBox;
    public KeypadUI UI;

    public int[] code;
    
    public delegate void Guess();
    public event Guess EventCorrectGuess;
    public event Guess EventWrongGuess;


    public void SetCode(int[] newCode) {
        code = newCode;
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
        UI.OpenUI();
    }

    private void OnPlayerExit(GameObject player) {
        player.GetComponent<PlayerActivate>().EventPlayerActivate -= PlayerActivate;
    }

}
