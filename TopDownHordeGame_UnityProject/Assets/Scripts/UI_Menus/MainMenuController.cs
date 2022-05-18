using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : Menu
{
    public GameObject settingsMenu;
    public void LoadPreGame() {
        SceneManager.LoadScene(1);
    }
    public void LoadHelpMenu() {
        SceneManager.LoadScene(2);
    }
    public void OpenSettings() {
        settingsMenu.SetActive(true);
    }
    public void ExitGame() {
        Debug.Log("There is no escape");
        Application.Quit();
    }
}
