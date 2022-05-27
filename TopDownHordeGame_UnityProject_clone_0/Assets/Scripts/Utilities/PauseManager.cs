using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : Menu
{
    [SerializeField] private string SceneLoadOnQuitGame;
    public GameObject menuObj;
    public Timer timer;

    public static PauseManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        timer = GetComponent<Timer>();
    }

    public delegate void PauseStateChange(bool isPaused);
    public event PauseStateChange EventPauseStateChange;

    private bool isPaused = false;
    public bool IsPaused() { return isPaused; }

    public void PauseButtonPress() {
        if (!isPaused)
            PauseMenu();
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

    public void PauseMenu() {
        //Debug.Log("PAUSE");
        menuObj.SetActive(true);
        if (isPaused) {
            Debug.Log("Can't pause. Already paused");
            return;
        }
        Time.timeScale = 0;
        isPaused = true;
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
        if (EventPauseStateChange != null) { EventPauseStateChange.Invoke(isPaused); }
    }

    public void ClosePauseMenu() {
        //Debug.Log("UNPAUSE");
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
        ClosePauseMenu();
        SaveData.Save();
        SceneManager.LoadScene(SceneLoadOnQuitGame);
    }

    public override void OnCancel() {
        base.OnCancel();
        timer.CreateTimer(2 * Time.deltaTime, UnpauseWithCheck);
    }
    public void UnpauseWithCheck() {
        if (isPaused)
            ClosePauseMenu();
    }

}
