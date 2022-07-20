
//This class is what is encoded into binary and saved to a file

[System.Serializable]
public class Save{

    //Settings
    public float settings_volumeSFX;
    public float settings_volumeMusic;

    //Leaderboard
    public int leaderboard_totalKills;
    public int leaderboard_mostKills; 
    public int leaderboard_totalMoneyEarned;
    public int leaderboard_mostMoneyEarned;
    public int leaderboard_totalScore;
    public int leaderboard_mostScore;
    public int leaderboard_highestRound;

    //CatCafe
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;


    public Save(SaveData data) {
        //Settings
        settings_volumeSFX = data.settings_volumeSFX;
        settings_volumeMusic = data.settings_volumeMusic;
        //LeaderBoard
        leaderboard_totalKills = data.leaderboard_totalKills;
        leaderboard_mostKills = data.leaderboard_mostKills;
        leaderboard_totalMoneyEarned = data.leaderboard_totalMoneyEarned;
        leaderboard_mostMoneyEarned = data.leaderboard_mostMoneyEarned;
        leaderboard_totalScore = data.leaderboard_totalScore;
        leaderboard_mostScore = data.leaderboard_mostScore;
        leaderboard_highestRound = data.leaderboard_highestRound;
        //Cat Cafe
        catCafe_code = data.catCafe_code;
        catCafe_unlockedDigits = data.catCafe_unlockedDigits;
        catCafe_unlockedElevator = data.catCafe_unlockedElevator;
    }
}
