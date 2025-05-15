using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class InteractWithData
{
    public string folderPath;
    public string filePath;

    public InteractWithData()
    {
        folderPath = Application.dataPath + "/Data";
        filePath = folderPath + "/ranking.xml";
        Debug.Log(folderPath);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Directory created");
        }
    }

    public void SaveData(string name, int score)
    {
        List<Data> entries = LoadEntries();
        Debug.Log(entries);

        // Add the new entry
        entries.Add(new Data(name, score));


        // Serialize the list (Like... Translate)
        XmlSerializer serializer = new XmlSerializer(typeof(List<Data>));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, entries);
        }

        Debug.Log("Saved game to: " + filePath);
    }

    public List<Data> LoadEntries()
    {
        // If the file already exists, deserialize and return its content
        if (File.Exists(filePath))
        {
            Debug.Log("File exists, serializing");
            XmlSerializer serializer = new XmlSerializer(typeof(List<Data>));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (List<Data>)serializer.Deserialize(stream);
            }
        }
        else
        {
            // File doesn't exist, so we create a new empty list and save it as XML
            Debug.Log("File does not exist, creating a new one");
            List<Data> emptyList = new List<Data>();

            // Create serializer for the list
            XmlSerializer serializer = new XmlSerializer(typeof(List<Data>));

            // Create the file and write the empty list
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, emptyList);
            }

            Debug.Log("Created new ranking file at: " + filePath);
            return emptyList;
        }
    }
    
    [System.Serializable]
    public class Data
    {
        public string name;
        public int score;

        // Parameterless constructor required for XmlSerializer
        public Data() { }

        public Data(string name, int score)
        {
            this.name = name;
            this.score = score;
        }

        // Override ToString to customize the output when logged
        public override string ToString()
        {
            return $"Name: {name}, Score: {score}";
        }
    }
}
