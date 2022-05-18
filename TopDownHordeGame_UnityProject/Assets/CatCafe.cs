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

    public List<Pickup> codePickups;
    private List<Pickup> availablePickups;

    private void Start() {
        //Load saved data
        keypadCode = SaveData.instance.catCafe_code;
        unlockedDigits = SaveData.instance.catCafe_unlockedDigits;
        unlockedElevator = SaveData.instance.catCafe_unlockedElevator;

        //Set up elevator keypad
        keypad.SetCode(keypadCode);
        keypad.SetUnlockedDigits(unlockedDigits);
        keypad.EventCorrectGuess += CorrectCodeEntered;

        //set up code pickups
        for (int i = 0; i < codePickups.Count; i++) {
            codePickups[i].EventOnCollect += UnlockDigit;
            codePickups[i].gameObject.SetActive(false);
        }
        availablePickups = codePickups;
        RoundController.instance.EventRoundChange += OnRoundChange;
    }

    public void UnlockDigit() {
        unlockedDigits++;
        SaveData.instance.catCafe_unlockedDigits = unlockedDigits;
        keypad.SetUnlockedDigits(unlockedDigits);
    }
    private void CorrectCodeEntered() {
        OpenElavatorArea();
    }

    public void SpawnCodePickup() {
        int choice = Random.Range(0, availablePickups.Count);
        availablePickups[choice].gameObject.SetActive(true);
        availablePickups.RemoveAt(choice);
    }

    public void OpenElavatorArea() {
        elavatorCover.SetActive(false);
        unlockedElevator = true;
        SaveData.instance.catCafe_unlockedElevator = true;
    }


    private void OnDestroy() {
        RoundController.instance.EventRoundChange -= OnRoundChange;
    }
    private void OnRoundChange(int round) {
        if (round >= 10 && round % 10 == 0)
            SpawnCodePickup();
    }
}
