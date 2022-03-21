using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadPreGame() {
        SceneManager.LoadScene(1);
    }
    public void LoadHelpMenu() {
        SceneManager.LoadScene(2);
    }
    public void ExitGame() {
        Debug.Log("There is no escape");
        Application.Quit();
    }
}
