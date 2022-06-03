using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : Menu
{

    public void LoadHelpMenu() {
        SceneManager.LoadScene(2);
    }
    public void ExitGame() {
        Debug.Log("There is no escape");
        Application.Quit();
    }
}
