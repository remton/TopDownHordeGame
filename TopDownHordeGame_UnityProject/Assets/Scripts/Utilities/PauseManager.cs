using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Mirror;

public class PauseManager : NetworkBehaviour
{
    public PauseMenu menu;
    public static PauseManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public delegate void PauseStateChange(bool isPaused);
    public event PauseStateChange EventPauseStateChange;

    private bool isPaused = false;
    public bool IsPaused() { return isPaused; }

    public void PauseButtonPress() {
        if (!isPaused) {
            PauseCMD(true);
        }
        else {
            PauseCMD(false);
        }
    }

    //Anyone can tell the server to pause
    [Command(requiresAuthority = false)]
    private void PauseCMD(bool paused) {
        PauseRPC(paused);
    }

    //Server tells all clients to pause the game
    [ClientRpc]
    private void PauseRPC(bool paused) {
        isPaused = paused;
        if (isPaused) {
            menu.OpenMenu();
            PauseTime();
        }
        else {
            menu.CloseMenu();
            UnpauseTime();
        }
    }

    public void PauseTime() {
        Time.timeScale = 0;
        isPaused = true;
        if (EventPauseStateChange != null) { EventPauseStateChange.Invoke(isPaused); }
    }

    public void UnpauseTime() {
        Time.timeScale = 1;
        isPaused = false; 
        if (EventPauseStateChange != null) { EventPauseStateChange.Invoke(isPaused); }
    }
}
