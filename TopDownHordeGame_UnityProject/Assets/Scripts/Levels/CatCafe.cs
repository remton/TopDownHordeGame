using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CatCafe : NetworkBehaviour
{
    public static string LevelName = "CatCafe";
    public static int codeLength = 4;
    [TextArea]
    public string unlockDigitMessage;

    public GameObject elavatorCover;
    public Keypad keypad;

    public int[] keypadCode;
    public int unlockedDigits;
    public bool unlockedElevator;

    public List<Pickup> codePickups;
    private List<Pickup> availablePickups;

    [SerializeField] private UnlockPopup unlockPopup;

    private void Start() {
        RoundController.instance.EventRoundChange += OnRoundChange;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        SceneLoader.instance.AddClientsLoad(OnAllClientsLoaded);
    }
    [Server]
    private void OnAllClientsLoaded() {
        //Load saved data
        keypadCode = SaveData.instance.catCafe_code;
        unlockedDigits = SaveData.instance.catCafe_unlockedDigits;
        unlockedElevator = SaveData.instance.catCafe_unlockedElevator;

        SetupPickupsRPC();
        SetUpKeypadRPC(keypadCode, unlockedDigits, unlockedElevator);
    }
    [ClientRpc]
    private void SetupPickupsRPC() {
        for (int i = 0; i < codePickups.Count; i++) {
            if(isServer)
                codePickups[i].EventOnCollect += UnlockDigit;
            codePickups[i].Activate(false);
        }
        availablePickups = codePickups;
    }
    [ClientRpc]
    private void SetUpKeypadRPC(int[] code, int numUnlocked, bool isUnlocked) {
        keypadCode = code;
        unlockedDigits = numUnlocked;
        unlockedElevator = isUnlocked;
        keypad.SetLockout(false);
        keypad.SetCode(keypadCode);
        keypad.SetUnlockedDigits(unlockedDigits);
        keypad.EventCorrectGuess += CorrectCodeEntered;
        keypad.EventWrongGuess += WrongCodeEntered;
    }
    [ClientRpc]
    private void UpdateKeypadValues(int[] code, int numUnlocked, bool isUnlocked) {
        keypadCode = code;
        unlockedDigits = numUnlocked;
        unlockedElevator = isUnlocked; 
        keypad.SetCode(keypadCode);
        keypad.SetUnlockedDigits(unlockedDigits);
    }

    [Server]
    public void UnlockDigit() {
        unlockPopup.Activate(unlockDigitMessage);
        unlockedDigits++;
        SaveData.instance.catCafe_unlockedDigits = unlockedDigits;
        keypad.SetUnlockedDigits(unlockedDigits);
        UpdateKeypadValues(keypadCode, unlockedDigits, unlockedElevator);
    }

    private void CorrectCodeEntered() {
        OpenElavatorCMD();
    }
    private void WrongCodeEntered() {
        keypad.SetLockout(true);
    }

    [Server]
    public void SpawnCodePickup() {
        int choice = Random.Range(0, availablePickups.Count);
        SpawnCodePickupRPC(choice);
    }
    [ClientRpc]
    public void SpawnCodePickupRPC(int choice) {
        availablePickups[choice].Activate(true);
        availablePickups.RemoveAt(choice);
    }

    [Command(requiresAuthority = false)]
    public void OpenElavatorCMD() {
        SaveData.instance.catCafe_unlockedElevator = true;
        OpenElevatorRPC();
    }
    [ClientRpc]
    private void OpenElevatorRPC() {
        elavatorCover.SetActive(false);
        unlockedElevator = true;
    }

    private void OnDestroy() {
        if(isServer)
            RoundController.instance.EventRoundChange -= OnRoundChange;
    }

    private void OnRoundChange(int round) {
        if (keypad.IsLockedOut())
            keypad.SetLockout(false);
        if (isServer) {
            if (round >= 10 && round % 10 == 0)
                if(unlockedDigits < codeLength)
                    SpawnCodePickup();
        }
    }
}
