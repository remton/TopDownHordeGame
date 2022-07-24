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

    private bool canPause = true;
    
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
        if (!canPause)
            return;
        if (context.action.triggered == true) {
            PauseManager.instance.PauseButtonPress();
        }
    }

    public void DisablePause() {
        canPause = false;
    }
    public void EnablePause() {
        canPause = true;
    }

    private void PausePlayer() {
        movement.isPaused = true;
        activate.isPaused = true;
        weapon.isPaused = true;
        timer.SavePauseState();
        timer.PauseAll();
    }
    private void UnPausePlayer() {
        movement.isPaused = false;
        activate.isPaused = false;
        weapon.isPaused = false;
        timer.LoadPauseState();
    }
}
