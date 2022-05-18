//This class handles saving and loading data

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    const string FILEPATH = "/game.save";

    public static void Save(SaveData data) {

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + FILEPATH;
        FileStream stream = new FileStream(path, FileMode.Create);

        Save save = new Save(data);

        formatter.Serialize(stream, save);
        stream.Close();
        Debug.Log("Saved to: " + path);
    }
    public static Save LoadSave() {
        string path = Application.persistentDataPath + FILEPATH;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Save save;
            if (stream.Length > 0)
                save = formatter.Deserialize(stream) as Save;
            else
                save = null;

            stream.Close();
            return save;
        }
        else {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }
    public static void DeleteSave() {
        string path = Application.persistentDataPath + FILEPATH;
        File.Delete(path);
        Debug.Log("Deleted " + path);
    }
}
