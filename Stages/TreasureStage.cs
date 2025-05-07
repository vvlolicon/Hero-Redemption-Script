using Assets.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TreasureStageData;

public class TreasureStage : StageSettings
{
    [SerializeField] List<LootBox> lootBoxes = new List<LootBox>();
    private void Start()
    {
        
    }

    public override void OnEnterStage()
    {
        base.OnEnterStage();
        ClearStage();
    }

    public void SetStage(bool b1, bool b2, bool b3, List<bool> lb,
        List<LootBoxData> newLootBoxesData)
    {
        SetStage(b1, b2, b3, lb);
        for(int i = 0; i < newLootBoxesData.Count; i++)
        {
            var newLootBoxData = newLootBoxesData[i];
            var newLootBoxItems = newLootBoxData.lootBoxItems;
            lootBoxes[i].SetBox(newLootBoxItems, newLootBoxData.isOpened);
        }
    }

    public List<LootBoxData> GetLootBoxesData()
    {
        List<LootBoxData> lootBoxesData = new ();
        foreach(var lootBox in lootBoxes)
        {
            LootBoxData newBox = new LootBoxData();
            newBox.lootBoxItems = lootBox.GetItems();
            newBox.isOpened = lootBox.IsBoxOpened();
            lootBoxesData.Add(newBox);
        }
        return lootBoxesData;
    }
}

[System.Serializable]
public class TreasureStageData
{
    public string stageID;
    public List<string> childStageIDs;
    public List<bool> pickedPickups;

    public bool isLocked;
    public bool isDiscovered;
    public bool isCleared;
    public List<LootBoxData> lootBoxesData;

    [System.Serializable]
    public struct LootBoxData
    {
        public List<ItemData> lootBoxItems;
        public bool isOpened;
    }

    public void SerializeData()
    {
        foreach(var lootBoxData in lootBoxesData)
        {
            lootBoxData.lootBoxItems.SerializeItemList();
        }
    }

    public void RebuildItemReferences()
    {
        foreach (var lootBoxData in lootBoxesData)
        {
            lootBoxData.lootBoxItems.RebuildItemList();
        }
    }
}
