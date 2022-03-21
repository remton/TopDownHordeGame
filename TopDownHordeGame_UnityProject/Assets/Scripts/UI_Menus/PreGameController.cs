using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreGameController : MonoBehaviour
{
    public int maxPlayers;
    public int numPlayers;
    public Text numPlayerTxt;

    private void Awake() {
        Cursor.visible = true; // Make sure we can see the cursor since it is made invisible and replaced by the reticle during gameplay
    }

    private void Start() {
        numPlayerTxt.text = numPlayers.ToString();
        numPlayers = GameSettings.instance.numPlayers;
        numPlayerTxt.text = numPlayers.ToString();
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
        //Loads cat cafe index 2
        SceneManager.LoadScene("CatCafe");
    }
    public void LoadMainMenu() {
        GameSettings.instance.numPlayers = numPlayers;
        //MainMenuIndex 0
        SceneManager.LoadScene("MainMenu");
    }
}
