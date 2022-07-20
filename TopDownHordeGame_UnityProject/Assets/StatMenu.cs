using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatMenu : Menu
{
    public Text mostKills;
    public Text totalKills;
    public Text mostMoney;
    public Text totalMoney;
    public Text mostScore;
    public Text totalScore;
    public Text highestRound;

    public Menu mainMenu;
    public override void Open() {
        base.Open();
        UpdateText();
    }
    public override void Close() {
        base.Close();
        mainMenu.Open();
    }
    private void UpdateText() {
        SaveData data = SaveData.instance;
        mostKills.text = "Most Kills: " + data.leaderboard_mostKills;
        totalKills.text = "Total Kills: " + data.leaderboard_totalKills;
        mostMoney.text = "Most Money: $" + data.leaderboard_mostMoneyEarned;
        totalMoney.text = "Total Money Earned: $" + data.leaderboard_totalMoneyEarned;
        mostScore.text = "Most Score: " + data.leaderboard_mostScore;
        totalScore.text = "Total Score: " + data.leaderboard_totalScore;
        highestRound.text = "Highest Round: " + data.leaderboard_highestRound;
    }
}
