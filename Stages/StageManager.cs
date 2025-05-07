using Assets.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton_LastIn<StageManager>
{
    private Dictionary<string, StageSettings> stageRegistry = new();
    [SerializeField] string LevelPrefix;

    private void Start()
    {
        RegisterAllStages();
    }

    void RegisterAllStages()
    {
        foreach (StageSettings stage in FindObjectsOfType<StageSettings>())
        {
            string stageName = LevelPrefix +"_";
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

    public void RestoreStages(List<object> savedStages, string curStageID)
    {
        foreach (var savedData in savedStages)
        {
            if(savedData is GeneralStageData savedStage)
            {
                if (stageRegistry.TryGetValue(savedStage.stageID, out StageSettings stage))
                {
                    stage.SetStage(savedStage.isLocked,
                                 savedStage.isDiscovered,
                                 savedStage.isCleared,
                                 savedStage.pickedPickups);
                }
            }
            else if(savedData is TreasureStageData savedTreasureStage)
            {
                savedTreasureStage.RebuildItemReferences();
                if (stageRegistry.TryGetValue(savedTreasureStage.stageID, out StageSettings stage))
                {
                    if(stage is TreasureStage treasureStage)
                    {
                        treasureStage.SetStage(savedTreasureStage.isLocked,
                                 savedTreasureStage.isDiscovered,
                                 savedTreasureStage.isCleared,
                                 savedTreasureStage.pickedPickups,
                                 savedTreasureStage.lootBoxesData);
                    }
                }
            }
        }
        if (!string.IsNullOrEmpty(curStageID))
        {
            StageMapController stageController = UI_Controller.GetUIScript<StageMapController>();
            if(stageRegistry.TryGetValue(curStageID, out StageSettings curstage))
            {
                Debug.Log("Current stage: " + curstage.gameObject);
                stageController.SetCurStage(curstage);
            }
        }
    }
}
