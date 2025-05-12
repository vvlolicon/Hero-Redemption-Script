using Assets.SaveSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public static class LevelManager
{
    public static int CurLevelScene = 0;
    public static void OnExitGame()
    {
        Application.Quit();
    }

    public static void LoadLevel(int levelSceneIndex)
    {
        PlayerData playerData = SaveSystem.GatherPlayerData();
        List<object> objs = new() { playerData };
        SaveSystem.SaveStageSettings();
        objs.AddRange(SaveSystem.savedStages);
        SceneLoader.Instance.LoadScene(levelSceneIndex, () =>
        {
            EnterNewLevel(objs.ToArray());
        });
    }

    public static void LoadDataToGame(object[] saveGameDatas)
    {
        UI_Controller.Instance.Initialize();
        if (saveGameDatas != null)
        {
            List<object> savedStages = new();
            string curStageID = "";
            foreach (object saveGameData in saveGameDatas)
            {
                if (saveGameData is PlayerData playerData)
                {
                    Debug.Log($"Find Game Data: " + saveGameData.ToString());
                    playerData.DeserializeData();
                    curStageID = playerData.curStageID;
                    var executor = PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>();
                    executor.LoadPlayerData(playerData);
                }
                if (saveGameData is GeneralStageData)
                    savedStages.Add(saveGameData);
                if (saveGameData is TreasureStageData)
                    savedStages.Add(saveGameData);
            }
            if (savedStages.Count > 0)
            {
                StageManager.Instance.RestoreStages(savedStages, curStageID);
            }
        }
        else
        {
            Debug.LogWarning("Cannot find any data");
        }
        AfterGameInit();
    }
    public static void EnterNewLevel(object[] saveGameDatas)
    {
        UI_Controller.Instance.Initialize();
        if (saveGameDatas != null)
        {
            List<object> savedStages = new();
            foreach (object saveGameData in saveGameDatas)
            {
                if (saveGameData is PlayerData playerData)
                {
                    var executor = PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>();
                    executor.LoadPlayerDataNoTransport(playerData);
                    var stageMapContr = UI_Controller.GetUIScript<StageMapController>();
                    Transform startPoint = stageMapContr.StartStage.EnteredPoint;
                    executor.TransportPlayerTo(startPoint);
                }
                if (saveGameData is GeneralStageData)
                    savedStages.Add(saveGameData);
                if (saveGameData is TreasureStageData)
                    savedStages.Add(saveGameData);
            }
            if (savedStages.Count > 0)
            {
                StageManager.Instance.SetStagesFromData(savedStages);
            }
        }
        else
        {
            Debug.LogWarning("Cannot find any data");
        }
        AfterGameInit();
    }

    public static void StartNewGame()
    {
        UI_Controller.Instance.Initialize();
        PlayerStateExecutor executor = PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>();
        executor.InitializePlayer();
        StageMapController stageController = UI_Controller.GetUIScript<StageMapController>();
        stageController.Init();
        StageManager.Instance.InitStages();
        UI_Controller.Instance.SetUIActive(UI_Window.TutorialUI, true);
        PlayerInputData.Instance.EnableAllInput(false);
        AfterGameInit();
    }

    static void AfterGameInit()
    {
        VolumeMaster volumeMaster = UI_Controller.Instance.PauseMenu.GetComponent<VolumeMaster>();
        volumeMaster.ResetSliders();
        UI_Controller.Instance.SetUIActive(UI_Window.StageMap, false);
    }
}