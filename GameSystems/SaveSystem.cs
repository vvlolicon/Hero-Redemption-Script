using Assets.SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

    public static void SaveGame(string fileName)
    {
        PlayerBackpack PlayerBackpack = PlayerCompManager.TryGetPlayerComp<PlayerBackpack>();
        PlayerStateExecutor PlayerExecutor = PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>();
        List<object> saveData = new List<object>();
        PlayerData playerData = new PlayerData();
        playerData.playerLevel = PlayerBackpack.PlayerLevel;
        playerData.playerMoney = PlayerBackpack.PlayerOwnedMoney;
        playerData.playerInventory = PlayerBackpack.GetPlayerBackpackItems();
        playerData.playerEquipment = PlayerBackpack.GetPlayerEquippedItems();
        playerData.playerHotbar = PlayerBackpack.GetPlayerHotbarItems();
        playerData.playerPos = PlayerBackpack.gameObject.transform.position;
        var stageMapContr = UI_Controller.GetUIScript<StageMapController>();
        var curStage = stageMapContr.curStage;
        if (curStage == null)
        {
            curStage = stageMapContr.StartStage;
        }
        playerData.curStageID = curStage.stageID;
        playerData.combatStats = PlayerExecutor.PlayerCombatStats.GetAllStats();
        playerData.extraStats = PlayerExecutor.ExtraStats.GetAllStats();
        playerData.SerializeData();
        saveData.Add(playerData);
        saveData.AddRange(StageManager.Instance.GetSavedStages());

        Initialize();
        SaveGame(fileName, saveData.ToArray());
        playerData.DeserializeData();// deserialize item data to prevent wired things happen
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

        // convert data to json synchronously
        Task[] getStringTasks = new Task[datas.Length];
        List<string> jsonStrings = new List<string>();
        for (int i = 0; i < getStringTasks.Length; i++)
        {
            var obj = datas[i];
            getStringTasks[i] = new Task(() =>
            {
                StringBuilder sb = new();
                sb.AppendLine(obj.GetType().AssemblyQualifiedName);
                sb.AppendLine(JsonUtility.ToJson(obj));
                sb.AppendLine(SEPERATOR);
                jsonStrings.Add(sb.ToString());
            });
            getStringTasks[i].Start();
        }
        Task.WaitAll(getStringTasks);
        // write data into file
        using (StreamWriter streamWriter = new StreamWriter(filePath, false))
        {
            foreach (string jsonString in jsonStrings)
            {
                streamWriter.WriteLine(jsonString);
            }
        }
    }

    public static object[] LoadGame(string filePath)
    {
        if (File.Exists(filePath))
        {
            Debug.Log("Load game from file at " + filePath);
            List<object> objects = new List<object>();
            string[] allLines = File.ReadAllLines(filePath);
            List<string> jsonStrings = new List<string>();
            List<Type> types = new List<Type>();

            // First pass: Collect JSON strings and types
            StringBuilder currentJson = new StringBuilder();
            Type currentType = null;

            foreach (string line in allLines)
            {
                if (line == SEPERATOR)
                {
                    if (currentType != null && currentJson.Length > 0)
                    {
                        types.Add(currentType);
                        jsonStrings.Add(currentJson.ToString());
                        currentType = null;
                        currentJson.Clear();
                    }
                }
                else if (currentType == null)
                {
                    currentType = Type.GetType(line);
                }
                else
                {
                    currentJson.AppendLine(line);
                }
            }

            // Second pass: Convert JSON strings to objects in parallel
            Task<object>[] getObjectTasks = new Task<object>[jsonStrings.Count];
            for (int i = 0; i < getObjectTasks.Length; i++)
            {
                var jsonString = jsonStrings[i];
                var type = types[i];
                getObjectTasks[i] = new Task<object>(() => JsonUtility.FromJson(jsonString, type));
                getObjectTasks[i].Start();
            }

            // Wait for all tasks to complete and collect results
            Task.WaitAll(getObjectTasks);
            foreach (var task in getObjectTasks)
            {
                objects.Add(task.Result);
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
