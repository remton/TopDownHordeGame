using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreenController : MonoBehaviour
{
    private Timer timer;
    private GameOverData data;
    public float timeUntilLoadNextScene;


    [Mirror.Scene]
    public string mainMenuScene = "";
    [Mirror.Scene]
    public string lobbyScene = "";

    public List<GameObject> statDisplays;
    public List<Text> nameTxts;
    public List<Text> scoreTxts;
    public List<Text> killTxts;
    public List<Text> moneyTxts;
    public Text roundTxt;

    private void Start() {
        timer = GetComponent<Timer>();
        data = GameOverData.instance;
        int displayCount;
        if(statDisplays.Count < data.numPlayers) {
            Debug.Log("TOO MANY PLAYERS TO DISPLAY");
            displayCount = statDisplays.Count;
        }
        else {
            displayCount = data.numPlayers;
        }

        for (int i = 0; i < displayCount; i++) {
            statDisplays[i].SetActive(true);
            nameTxts[i].text = data.playerNames[i];
            scoreTxts[i].text = "Score: " + data.scores[i].ToString();
            killTxts[i].text = "Kills: " + data.kills[i].ToString();
            moneyTxts[i].text = "Money Earned: $" + data.moneys[i].ToString();
        }
        roundTxt.text = "Round " + data.round.ToString();
        timer.CreateTimer(timeUntilLoadNextScene, LoadNextScene);
    }

    public void LoadNextScene() {
        //this is an online game
        if (MyNetworkManager.instance.isNetworkActive) {
            if (PlayerConnection.myConnection.isServer) {
                MyNetworkManager.instance.EndGame();
                MyNetworkManager.instance.ChangeScene(lobbyScene);
            }
        }
        else {
            //this is an offline game
            SceneManager.LoadScene(mainMenuScene);
        }

       
    }
}
