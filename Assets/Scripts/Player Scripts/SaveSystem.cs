using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{


    /// <summary>
    /// Save player data by using a binary formatter and a file stream
    /// </summary>
    /// <param name="player"></param>
    public static void SavePlayer (Player player)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(player);
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
    }


    /// <summary>
    /// Load player data by translating from the binary formatter
    /// </summary>
    public static PlayerData LoadPlayer ()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);
            PlayerData data = binaryFormatter.Deserialize(fileStream) as PlayerData;
            fileStream.Close();
            return data;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
