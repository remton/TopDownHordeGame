
//This class is what is encoded into binary and saved to a file

[System.Serializable]
public class Save{
    public string testStr;

    public Save(SaveData data) {
        testStr = data.test;
    }
}
