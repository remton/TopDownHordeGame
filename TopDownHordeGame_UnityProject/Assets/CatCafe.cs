using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatCafe : MonoBehaviour
{
    public static string LevelName = "CatCafe";

    public GameObject elavatorCover;
    public Keypad keypad;

    public int[] keypadCode;
    public int unlockedDigits;
    public bool unlockedElevator;

    private void Start() {
        //Load saved data
        keypadCode = SaveData.instance.catCafe_code;
        unlockedDigits = SaveData.instance.catCafe_unlockedDigits;
        unlockedElevator = SaveData.instance.catCafe_unlockedElevator;

        //Set up elevator keypad
        keypad.SetCode(keypadCode);
        keypad.SetUnlockedDigits(unlockedDigits);
        keypad.EventCorrectGuess += CorrectCodeEntered;
    }

    public void UnlockDigit() {
        unlockedDigits++;
        keypad.SetUnlockedDigits(unlockedDigits);
    }
    private void CorrectCodeEntered() {
        OpenElavatorArea();
    }

    public void OpenElavatorArea() {
        elavatorCover.SetActive(false);
        unlockedElevator = true;
        SaveData.instance.catCafe_unlockedElevator = true;
    }
}
