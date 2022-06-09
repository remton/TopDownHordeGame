using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverData : MonoBehaviour
{
    public static GameOverData instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Debug.Log("GameOverData already has an instance, one was destoyed");
            Destroy(gameObject);
        }
    }
    public int numPlayers = 0;
    public List<string> playerNames;
    public List<int> scores;
    public List<int> kills;
    public List<int> moneys;

    public void SetData(List<GameObject> players) {
        playerNames.Clear();
        scores.Clear();
        kills.Clear();
        moneys.Clear();
        numPlayers = players.Count;
        for (int i = 0; i < numPlayers; i++) {
            playerNames.Add("Player " + (i+1).ToString());
            scores.Add(players[i].GetComponent<PlayerStats>().GetTotalScore());
            kills.Add(players[i].GetComponent<PlayerStats>().GetTotalKills());
            moneys.Add(players[i].GetComponent<PlayerStats>().GetTotalMoney());
        }
    }


}
