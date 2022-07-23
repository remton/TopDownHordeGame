using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : Menu {

    [Mirror.Scene]
    public string helpScene;

    public override void OnCancel() {
        //Do nothing
    }
    public void LoadHelpMenu() {
        SceneManager.LoadScene(helpScene);
    }
    public void ExitGame() {
        Debug.Log("There is no escape");
        Application.Quit();
    }
}
