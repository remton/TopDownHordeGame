using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPauseControl : MonoBehaviour
{
    PlayerMovement movement;
    PlayerWeaponControl weapon;
    PlayerActivate activate;
    Timer timer;

    private void Awake() {
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponControl>();
        activate = GetComponent<PlayerActivate>();
        timer = GetComponent<Timer>();
    }
    private void Start() {
        PauseManager.instance.EventPauseStateChange += OnPauseStateChange;
    }
    private void OnDestroy() {
        PauseManager.instance.EventPauseStateChange -= OnPauseStateChange;
    }

    public void OnPauseStateChange(bool isPaused) {
        if (isPaused) {
            PausePlayer();
        }
        else {
            UnPausePlayer();
        }
    }

    public void OnPauseButton(InputAction.CallbackContext context) {
        if (context.action.triggered == true) {
            PauseManager.instance.PauseButtonPress();
        }
    }
    private void PausePlayer() {
        movement.isDisabled = true;
        activate.isDisabled = true;
        weapon.isDisabled = true;
        timer.SavePauseState();
        timer.PauseAll();
    }
    private void UnPausePlayer() {
        movement.isDisabled = false;
        activate.isDisabled = false;
        weapon.isDisabled = false;
        timer.LoadPauseState();
    }
}
