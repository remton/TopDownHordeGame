using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PreGameController : Menu
{
    public int maxPlayers;
    public int numPlayers = 0;
    public Text numPlayerTxt;
    public List<GameObject> playerBoxes;
    private List<int> deviceIds;
    private List<InputDevice> devices;

    private void Awake() {
        devices = new List<InputDevice>();
        deviceIds = new List<int>();
        Cursor.visible = true; // Make sure we can see the cursor since it is made invisible and replaced by the reticle during gameplay
    }

    private void Start() {
        numPlayerTxt.text = numPlayers.ToString();
        numPlayers = 0;
        numPlayerTxt.text = numPlayers.ToString();
        //InputSystem.onDeviceChange += OnDeviceChange;
    }
    private void OnDestroy() {
        //InputSystem.onDeviceChange -= OnDeviceChange;
    }

    //public void OnDeviceChange(InputDevice device, InputDeviceChange change) {
    //    switch (change) {
    //        case InputDeviceChange.Added:
    //            break;
    //        case InputDeviceChange.Removed:
    //            break;
    //        case InputDeviceChange.Disconnected:
    //            break;
    //        case InputDeviceChange.Reconnected:
    //            break;
    //        case InputDeviceChange.Enabled:
    //            break;
    //        case InputDeviceChange.Disabled:
    //            break;
    //        case InputDeviceChange.UsageChanged:
    //            break;
    //        case InputDeviceChange.ConfigurationChanged:
    //            break;
    //        case InputDeviceChange.Destroyed:
    //            break;
    //        default:
    //            break;
    //    }
    //}

    protected override void Update() {
        base.Update();

        if(Gamepad.current != null) {
            Debug.Log("GAMEPAD");
        }

        if(Gamepad.current != null && !deviceIds.Contains(Gamepad.current.deviceId)) {
            if (Gamepad.current.startButton.IsPressed()) {
                deviceIds.Add(Gamepad.current.deviceId);
                devices.Add(Gamepad.current);
                GameSettings.instance.devices = devices;
                OnPlayerJoined();
            }
        }
        if(Keyboard.current != null && !deviceIds.Contains(Keyboard.current.deviceId)) {
            if (Keyboard.current.spaceKey.IsPressed()) {
                deviceIds.Add(Keyboard.current.deviceId);
                devices.Add(Keyboard.current);
                GameSettings.instance.devices = devices;
                OnPlayerJoined();
            }
        }
    }

    public void OnPlayerJoined() {
        playerBoxes[numPlayers].SetActive(true);
        IncreaseNumPlayers();
    }
    public void OnPlayerLeft() {
        DecreaseNumPlayers();
        playerBoxes[numPlayers].SetActive(false);
    }

    public void IncreaseNumPlayers() {
        if(numPlayers < maxPlayers) {
            numPlayers++;
            numPlayerTxt.text = numPlayers.ToString();
        }
        GameSettings.instance.numPlayers = numPlayers;
    }
    public void DecreaseNumPlayers() {
        if (numPlayers > 1) {
            numPlayers--;
            numPlayerTxt.text = numPlayers.ToString();
        }
        GameSettings.instance.numPlayers = numPlayers;
    }

    public void StartGame() {
        SceneManager.LoadScene("CatCafe");
    }
    public void LoadMainMenu() {
        GameSettings.instance.numPlayers = numPlayers;
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnCancel() {
        base.OnCancel();
        LoadMainMenu();
    }
}
