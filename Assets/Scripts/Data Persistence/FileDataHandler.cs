using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID)
    {
        // Use Path.Combine to account for different OS's having different path seperators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath)) 
        {
            try
            {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileID) 
    {
        // Use Path.Combine to account for different OS's having different path seperators
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);

        try
        {
            // Create the directory the file will be written to if it doesn't exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // Optionally encrypt the data
            if(useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e) 
        {
            Debug.Log("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictioonary = new Dictionary<string, GameData>();

        // Loop over all directory names in the directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            // Defensive programming - check if the data file exists
            // if it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            if(!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading profiles because it does not contain data:" + profileID); continue;
            }

            // Load the game data for this file
            GameData profileData = Load(profileID);
            
            // Defensive programming - ensure the profile data isn't null
            // Because if it is then something went wrong and we should let ourselves know
            if(profileData != null)
            {
                profileDictioonary.Add(profileID, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileID: " + profileID);
            }
        }

        return profileDictioonary;
    }

    // The below is a simple implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++) 
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
