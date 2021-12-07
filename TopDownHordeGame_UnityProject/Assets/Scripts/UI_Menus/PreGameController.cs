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
    public string sceneToLoad;

    private void Start() {
        numPlayerTxt.text = numPlayers.ToString();
    }

    public void IncreaseNumPlayers() {
        if(numPlayers <= maxPlayers) {
            numPlayers++;
            numPlayerTxt.text = numPlayers.ToString();
        }
    }
    public void DecreaseNumPlayers() {
        if (numPlayers > 1) {
            numPlayers--;
            numPlayerTxt.text = numPlayers.ToString();
        }
    }

    public void StartGame() {
        GameSettings.instance.numPlayers = numPlayers;
        SceneManager.LoadScene(sceneToLoad);
    }
}