
//This class is what is encoded into binary and saved to a file

[System.Serializable]
public class Save{

    //Settings
    public float settings_volumeMaster;
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

    //Challenges
    public bool[] challenge_unlocks;
    public bool[] challenge_completed;
    public int challenge_biggestFanKills;
    public int challenge_zathrakKills;
    public int challenge_LungsKills;
    public int challenge_hockEyeKills;

    //Modifiers
    public bool[] modifier_unlocks;

    //CatCafe
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;

    public Save(SaveData data) {
        //Settings
        settings_volumeMaster = data.settings_volumeMaster;
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
        //Challenges
        challenge_unlocks = data.challenge_unlocks;
        challenge_completed = data.challenge_completed;
        challenge_biggestFanKills = data.challenge_biggestFanKills;
        challenge_zathrakKills = data.challenge_zathrakKills;
        challenge_LungsKills = data.challenge_LungsKills;
        challenge_hockEyeKills = data.challenge_hockEyeKills;
        //Cat Cafe
        catCafe_code = data.catCafe_code;
        catCafe_unlockedDigits = data.catCafe_unlockedDigits;
        catCafe_unlockedElevator = data.catCafe_unlockedElevator;
        //ModifierUnlocks
        modifier_unlocks = data.modifier_unlocks;
    }
}
