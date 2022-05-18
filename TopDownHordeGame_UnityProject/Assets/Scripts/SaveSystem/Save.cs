
//This class is what is encoded into binary and saved to a file

[System.Serializable]
public class Save{

    //Settings
    public float settings_volumeSFX;
    public float settings_volumeMusic;

    //CatCafe
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;

    public Save(SaveData data) {
        //Settings
        settings_volumeSFX = data.settings_volumeSFX;
        settings_volumeMusic = data.settings_volumeMusic;

        //Cat Cafe
        catCafe_code = data.catCafe_code;
        catCafe_unlockedDigits = data.catCafe_unlockedDigits;
        catCafe_unlockedElevator = data.catCafe_unlockedElevator;
    }
}
