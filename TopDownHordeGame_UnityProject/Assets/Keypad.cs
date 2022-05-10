using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour {

    private HitBoxController hitBox;
    public KeypadUI UI;

    public List<int> code = new List<int>();
    public void SetCode(List<int> newCode) { code = newCode; }

    public delegate void Guess();
    public event Guess EventCorrectGuess;
    public event Guess EventWrongGuess;

    public List<int> numPressed;


    public void OnPressNum(int num) {
        numPressed.Add(num);
        UI.UpdateUI(numPressed);
    }

    public void OnSubmit() {
        if(numPressed == code) {
            if (EventCorrectGuess != null) { EventCorrectGuess.Invoke(); }
            UI.CloseUI();
        }
        else {
            if (EventWrongGuess != null) { EventWrongGuess.Invoke(); }
            Clear();
        }
    }
    public void Clear() {
        numPressed.Clear();
        UI.UpdateUI(numPressed);
    }

    private void Awake() {
        hitBox = GetComponent<HitBoxController>();
        hitBox.EventObjEnter += OnPlayerEnter;
        hitBox.EventObjExit += OnPlayerExit;
        UI.EventNumPressed += OnPressNum;
        UI.EventSubmitPressed += OnSubmit;
    }

    private void OnDestroy() {
        hitBox.EventObjEnter -= OnPlayerEnter;
        hitBox.EventObjExit -= OnPlayerExit;
        UI.EventNumPressed -= OnPressNum;
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
