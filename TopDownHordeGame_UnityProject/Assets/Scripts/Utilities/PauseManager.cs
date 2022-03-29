using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : Menu
{
    [SerializeField] private string SceneLoadOnQuitGame;
    public GameObject menuObj;

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
        if (isPaused)
            UnPause();
        else
            Pause();
    }

    public void Pause() {
        menuObj.SetActive(true);
        if (isPaused) {
            Debug.Log("Can't pause. Already paused");
            return;
        }
        Time.timeScale = 0;
        isPaused = true;
        if (EventPauseStateChange != null) { EventPauseStateChange.Invoke(isPaused); }
    }

    public void UnPause() {
        menuObj.SetActive(false);
        if (!isPaused) {
            Debug.Log("Can't unpause. Already unpaused");
            return;
        }
        Time.timeScale = 1;
        isPaused = false;
        if (EventPauseStateChange != null) { EventPauseStateChange.Invoke(isPaused); }
    }

    public void QuitToMainMenu() {
        UnPause();
        SceneManager.LoadScene(SceneLoadOnQuitGame);
    }

    public override void OnCancel() {
        base.OnCancel();
        if(isPaused)
            UnPause();
    }
}
