
//This class is what is encoded into binary and saved to a file

[System.Serializable]
public class Save{
    public int[] catCafe_code;
    public int catCafe_unlockedDigits;
    public bool catCafe_unlockedElevator;

    public Save(SaveData data) {
        catCafe_code = data.catCafe_code;
        catCafe_unlockedDigits = data.catCafe_unlockedDigits;
        catCafe_unlockedElevator = data.catCafe_unlockedElevator;
    }
}
