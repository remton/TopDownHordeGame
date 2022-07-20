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
    public int round;
    public List<string> playerNames;
    public List<int> scores;
    public List<int> kills;
    public List<int> moneys;

    public void SetData(List<GameObject> players, int round) {
        this.round = round; 
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
        SaveProgress();
    }

    public void SaveProgress() {
        SaveData save = SaveData.instance;
        //kills
        int totalkills = 0;
        for (int i = 0; i < kills.Count; i++) {
            totalkills += kills[i];
            if (kills[i] > save.leaderboard_mostKills)
                save.leaderboard_mostKills = kills[i];
        }
        save.leaderboard_totalKills += totalkills;
        //Money
        int totalMoney = 0;
        for (int i = 0; i < moneys.Count; i++) {
            totalMoney += moneys[i];
            if (moneys[i] > save.leaderboard_mostMoneyEarned)
                save.leaderboard_mostMoneyEarned = moneys[i];
        }
        save.leaderboard_totalMoneyEarned += totalMoney;
        //Score
        int totalScore = 0;
        for (int i = 0; i < scores.Count; i++) {
            totalScore += scores[i];
            if (scores[i] > save.leaderboard_mostScore)
                save.leaderboard_mostScore = scores[i];
        }
        save.leaderboard_totalScore += totalScore;
        //Round
        if (round > save.leaderboard_highestRound)
            save.leaderboard_highestRound = round;
        SaveData.instance = save;
        SaveData.Save();
    }
}
