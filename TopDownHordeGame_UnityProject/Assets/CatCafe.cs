using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCafe : MonoBehaviour
{
    const bool OPEN_ELAVATOR_ON_START = false;
    public GameObject elavatorCover;
    public Keypad keypad;


    private void Start() {
        if (OPEN_ELAVATOR_ON_START) {
            OpenElavatorArea();
        }
        int[] keypadCode = {1,2,3,4,5};
        keypad.SetCode(keypadCode);
        keypad.EventCorrectGuess += CorrectCodeEntered;
    }
    private void CorrectCodeEntered() {
        OpenElavatorArea();
    }

    public void OpenElavatorArea() {
        elavatorCover.SetActive(false);
    }
}
