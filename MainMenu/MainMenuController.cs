using Assets.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class MainMenuController : SingletonDDOL<MainMenuController>
{
    private void Start()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(gameObject, thisScene);
        StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
        {
            VolumeMaster volumeMaster = FindObjectOfType<VolumeMaster>();
            volumeMaster.InitializeSliders();
            DontDestroyOnLoad(gameObject);
        }));
    }
    public void OnClickPlaygame()
    {
        StartGamePreActions();
        StartCoroutine(LoadGameScene(()=> { StartNewGame(); }));
    }
    public void OnLoadGame(string savePath)
    {
        StartGamePreActions();
        StartCoroutine(LoadGameScene(() => {
            LoadGameFromSave(savePath);
        }));
    }
    public void OnReturnMainMenu()
    {
        StartCoroutine(ReturnToMainMenu());
    }
    public void OnClickExit()
    {
        Application.Quit();
    }

    void LoadGameFromSave(string saveGamePath)
    {
        object[] saveGameDatas = SaveSystem.LoadGame(saveGamePath);
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
                    //var player = PlayerCompManager.Player;
                    //Debug.Log($"player is {player}, player executor is {executor}");
                    //if (player == null || player.IsDestroyed())
                    //{
                    //    Debug.LogWarning("Player is null or destroyed");
                    //}
                    //if (executor == null || executor.IsDestroyed())
                    //{
                    //    Debug.LogWarning("Player executor is null or destroyed");
                    //}
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
        UI_Controller.Instance.Initialize();
        AfterGameInit();
    }
    


    void StartGamePreActions()
    {
        FindObjectOfType<VolumeMaster>().RemoveLisenersForSliders();
        Destroy(FindAnyObjectByType<AudioListener>().gameObject);
        Destroy(FindAnyObjectByType<EventSystem>().gameObject);
    }

    void StartNewGame()
    {
        PlayerStateExecutor executor = PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>();
        executor.InitializePlayer();
        StageMapController stageController = UI_Controller.GetUIScript<StageMapController>();
        stageController.Init();
        StageManager.Instance.InitStages();
        UI_Controller.Instance.Initialize();
        UI_Controller.Instance.SetUIActive(UI_Window.TutorialUI, true);
        PlayerInputData.Instance.EnableAllInput(false);
        AfterGameInit();
    }

    void AfterGameInit()
    {
        VolumeMaster volumeMaster = VolumeMaster.Instance;
        volumeMaster.ResetSliders();
    }

    IEnumerator LoadGameScene(Action actionAfterLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = true;
    
        while (!asyncLoad.isDone)
        {
            yield return new WaitForFixedUpdate();
        }
    
        yield return new WaitForSeconds(0.1f);
        actionAfterLoad();
    }

    IEnumerator ReturnToMainMenu()
    {
        FindObjectOfType<VolumeMaster>().RemoveLisenersForSliders();
        DontDestroyOnLoad(gameObject);
        // load new scene
        var asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForFixedUpdate(); //delay for a fixed update frame;
        }
        yield return new WaitForSeconds(0.2f);
        VolumeMaster volumeMaster = FindObjectOfType<VolumeMaster>();
        volumeMaster.ResetSliders();
    }
}
