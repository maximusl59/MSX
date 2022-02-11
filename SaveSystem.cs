using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void Save(LoginData loginData) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/login.sav";
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, loginData);
        fileStream.Close();
    }

    public static LoginData Load() {
        string path = Application.persistentDataPath + "/login.sav";
        if(File.Exists(path)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            LoginData loginData = binaryFormatter.Deserialize(fileStream) as LoginData;

            return loginData;
        } else {
            return null;
        }
    }

    public static void Delete() {
        string path = Application.persistentDataPath + "/login.sav";
        File.Delete(path);
    }
}
