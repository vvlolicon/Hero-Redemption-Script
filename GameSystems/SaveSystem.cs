using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    public static readonly string SAVE_FOLDER = Path.Combine(Application.dataPath, "Saves") + "/";
    public static readonly string SAVE_EXTENSION = ".save";
    const string SEPERATOR = "---";
    public static void Initialize()
    { // create save folder if it doesn't exist
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void SaveGame(string fileName, object[] datas)
    {
        string filePath = SAVE_FOLDER + fileName + SAVE_EXTENSION;
        int saveNum = 1;
        while (File.Exists(filePath))
        {
            filePath = SAVE_FOLDER + fileName + " (" + saveNum + ")" + SAVE_EXTENSION;
            saveNum++; 
        }

        using (StreamWriter sw = new StreamWriter(filePath, false))
        {
            foreach (object obj in datas)
            {
                string json = JsonUtility.ToJson(obj);
                sw.WriteLine(obj.GetType().AssemblyQualifiedName);
                sw.WriteLine(json);
                sw.WriteLine(SEPERATOR);
                Debug.Log($"Seriliazed obj json£º{json}");
            }
        }
        Debug.Log($"Successfully saved game to {filePath}");
    }

    public static object[] LoadGame(string filePath)
    {
        if (File.Exists(filePath))
        {
            Debug.Log("Load game from file at " + filePath);
            List<object> objects = new List<object>();
            string[] allLines = File.ReadAllLines(filePath);
            Type currentType = null;
            StringBuilder currentJson = new StringBuilder();

            foreach (string line in allLines)
            {
                if (line == SEPERATOR)
                {
                    if (currentType != null && currentJson.Length > 0)
                    {
                        objects.Add(JsonUtility.FromJson(currentJson.ToString(), currentType));
                        currentType = null;
                        currentJson.Clear();
                    }
                }
                else if (currentType == null)
                {
                    //Debug.Log($"reading save object type£º{line}");
                    currentType = Type.GetType(line);
                    //if (currentType != null)
                    //{
                    //    Debug.Log($"Type found: {currentType.FullName}");
                    //}
                    //else
                    //{
                    //    Debug.Log("Type not found.");
                    //}
                }
                else
                {
                    //Debug.Log($"reading save file json£º{line}");
                    currentJson.AppendLine(line);
                }
            }
            return objects.ToArray();
        }
        else return null;
    }

    public static string[] GetAllSavePaths(out List<string> fileNames)
    {
        Initialize();
        fileNames = new List<string>();

        string[] files = Directory.GetFiles(SAVE_FOLDER, "*" + SAVE_EXTENSION);
        foreach (string filePath in files)
        {
            //Debug.Log($"Find save file at {filePath}");
            string fileName = Path.GetFileName(filePath);
            string pureName = Path.GetFileNameWithoutExtension(fileName);
            fileNames.Add(pureName);
        }
        return files;
    }
}
