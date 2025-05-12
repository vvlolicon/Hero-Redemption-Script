using Assets.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton_LastIn<StageManager>
{
    private Dictionary<string, StageSettings> stageRegistry = new();
    [SerializeField] string LevelPrefix;

    private void Awake()
    {
        RegisterAllStages();
    }

    void RegisterAllStages()
    {
        foreach (StageSettings stage in FindObjectsOfType<StageSettings>())
        {
            string stageName = LevelPrefix + LEVEL_SPLITTER;
            if (!string.IsNullOrEmpty(stage.stageID))
            {
                stageName += stage.stageID;
            }
            else
            {
                stageName += stage.gameObject.name;
            }
            stage.stageID = stageName;
            //Debug.Log($"Registing ID {stageName} to {stage.gameObject.name}");
            stageRegistry[stageName] = stage;
        }
    }

    public void InitStages()
    {
        foreach (var stage in stageRegistry.Values)
        {
            stage.InitStage();
        }
    }

    public object[] GetSavedStages()
    {
        List<object> savedStages = new();
        foreach (var stage in stageRegistry.Values)
        {
            if(stage is TreasureStage treasureStage)
            {
                TreasureStageData stageData = new TreasureStageData();
                stageData.stageID = stage.stageID;
                stageData.isLocked = stage.IsStageLocked();
                stageData.isDiscovered = stage.IsStageDiscovered();
                stageData.isCleared = stage.IsStageCleared();
                stageData.lootBoxesData = treasureStage.GetLootBoxesData();
                stageData.pickedPickups = stage.GetIsPickupsPicked();
                savedStages.Add(stageData);
            }
            else
            {
                GeneralStageData stageData = new GeneralStageData();
                stageData.stageID = stage.stageID;
                stageData.isLocked = stage.IsStageLocked();
                stageData.isDiscovered = stage.IsStageDiscovered();
                stageData.isCleared = stage.IsStageCleared();
                stageData.pickedPickups = stage.GetIsPickupsPicked();
                savedStages.Add(stageData);
            }
        }
        return savedStages.ToArray();
    }

    public static StageSettings[] GetCurLevelStages() => FindObjectsOfType<StageSettings>();

    public static TreasureStageData[] GetStagesAsTreasureStageData(object[] savedStages)
    {
        List<TreasureStageData> treasureStages = new();
        foreach (var savedData in savedStages)
        {
            if (savedData is TreasureStageData savedTreasureStage)
            {
                treasureStages.Add(savedTreasureStage);
            }
        }
        return treasureStages.ToArray();
    }

    public static GeneralStageData[] GetStagesAsGeneralStageData(object[] savedStages)
    {
        List<GeneralStageData> generalStages = new();
        foreach (var savedData in savedStages)
        {
            if (savedData is GeneralStageData savedGeneralStage)
            {
                generalStages.Add(savedGeneralStage);
            }
        }
        return generalStages.ToArray();
    }

    public void RestoreStages(List<object> savedStages, string curStageID)
    {
        SetStagesFromData(savedStages);
        if (!string.IsNullOrEmpty(curStageID))
        {
            foreach (var stage in GetCurLevelStages())
            {
                if (stage.stageID == curStageID)
                {
                    StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
                    {
                        StageMapController stageController = UI_Controller.GetUIScript<StageMapController>();
                        stageController.SetCurStage(stage);
                    }));
                    break;
                }
            }
        }
    }

    public void SetStagesFromData(List<object> savedStages)
    {
        List<StageSettings> restoredStages = new();
        foreach (var savedData in savedStages)
        {
            if (savedData is GeneralStageData savedStage)
            {
                if (stageRegistry.TryGetValue(savedStage.stageID, out StageSettings stage))
                {
                    stage.SetStage(savedStage.isLocked,
                                 savedStage.isDiscovered,
                                 savedStage.isCleared,
                                 savedStage.pickedPickups);
                    restoredStages.Add(stage);
                }
            }
            else if (savedData is TreasureStageData savedTreasureStage)
            {
                savedTreasureStage.RebuildItemReferences();
                if (stageRegistry.TryGetValue(savedTreasureStage.stageID, out StageSettings stage))
                {
                    if (stage is TreasureStage treasureStage)
                    {
                        treasureStage.SetStage(savedTreasureStage.isLocked,
                                 savedTreasureStage.isDiscovered,
                                 savedTreasureStage.isCleared,
                                 savedTreasureStage.pickedPickups,
                                 savedTreasureStage.lootBoxesData);
                        restoredStages.Add(stage);
                    }
                }
            }
        }
    }

    public string GetStageFullID(string stageID) => LevelPrefix + LEVEL_SPLITTER + stageID;

    public const string LEVEL_SPLITTER = "_";

    public StageSettings GetStageByShortID(string stageID) => stageRegistry[GetStageFullID(stageID)];
}
