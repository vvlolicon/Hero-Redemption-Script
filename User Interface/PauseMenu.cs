using Assets.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerBackpack PlayerBackpack { get { return GameObjectManager.TryGetPlayerComp<PlayerBackpack>(); } }
    PlayerStateExecutor PlayerExecutor { get { return GameObjectManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    public void OnExitWindow()
    {
        Time.timeScale = 1;
        UI_Controller.OnClosableWindowExit(gameObject);
    }

    public void OnExitGame()
    {
        MainMenuController.Instance.OnReturnMainMenu();
    }

    public void OnSaveGame()
    {
        List<object> saveData = new List<object>();
        PlayerData playerData = new PlayerData();
        playerData.playerLevel = PlayerBackpack.PlayerLevel;
        playerData.playerMoney = PlayerBackpack.PlayerOwnedMoney;
        playerData.playerInventory = PlayerBackpack.GetPlayerBackpackItems();
        playerData.playerEquipment = PlayerBackpack.GetPlayerEquippedItems();
        playerData.playerHotbar = PlayerBackpack.GetPlayerHotbarItems();
        playerData.playerPos = PlayerBackpack.gameObject.transform.position;
        playerData.curStageID = UI_Controller.GetUIScript<StageMapController>().curStage.stageID;
        playerData.combatStats = PlayerExecutor.PlayerCombatStats.GetAllStats();
        playerData.extraStats = PlayerExecutor.ExtraStats.GetAllStats();
        playerData.SerializeData();
        saveData.Add(playerData);
        saveData.AddRange(StageManager.Instance.GetSavedStages());
        SaveSystem.Initialize();
        SaveSystem.SaveGame("New Save", saveData.ToArray());
        playerData.DeserializeData();// deserialize item data
        OnExitWindow();
    }
}
