using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    public int PlayerBackpackSize = 20;
    List<ItemData> _playerInventory;
    ItemData[] _playerEquipments = new ItemData[8];
    ItemData[] _playerHotbar = new ItemData[6];

    [SerializeField] List<Item> _playerOwnedItems;
    [SerializeField] Item[] _playerEquippedItems = new Item[8];
    [SerializeField] Item[] _playerHotbarItems = new Item[6];
    [HideInInspector] public int PlayerLevel = 1;
    [HideInInspector] public int PlayerOwnedMoney = 0;
    //public int PlayerExp;

    public event CombatBuffHandler.OnStatsChangedDelegate OnStatsChanged;

    private void Awake()
    {
        List<ItemData> newList = new List<ItemData>(PlayerBackpackSize);
        for (int i = 0; i < PlayerBackpackSize; i++)
        {
            if (i < _playerOwnedItems.Count)
                newList.Add(_playerOwnedItems[i].GetItemDataClone());
            else
                newList.Add(null);
        }
        _playerInventory = newList;
        for(int i = 0; i < _playerEquippedItems.Length; i++)
        {
            if (_playerEquippedItems[i] != null)
                _playerEquipments[i] = _playerEquippedItems[i].GetItemDataClone();
            else
                _playerEquipments[i] = null;
        }
        for (int i = 0; i < _playerHotbarItems.Length; i++)
        { 
            if (_playerHotbarItems[i] != null)
                _playerHotbar[i] = _playerHotbarItems[i].GetItemDataClone();
            else
                _playerHotbar[i] = null;
        }
    }

    public void AddEnemyDrops(EnemyDropData dropData)
    {
        //PlayerExp += dropData.EXP_drop;
        PlayerOwnedMoney += dropData.Money_drop;
        List<Item> dropItems = dropData.GetDropItems();
        if (dropItems.Count > 0)
        {
            foreach (Item item in dropItems)
            {
                if (!AddItemToPlayerBackpack(item.GetItemDataClone()))
                {
                    Debug.Log("Backpack is full, dropping " + item.itemName + " on the ground");
                }
            }
        }
    }

    public bool AddItemToPlayerBackpack(ItemData item)
    {
        bool AddItem = true;
        if (item.itemType == ItemType.Consumable && _playerInventory.Count > 0)
        {
            int stackLeft = item.curStack;
            foreach (ItemData backpackItem in _playerInventory)
            {
                if (backpackItem == null) continue;
                if (backpackItem.itemID == item.itemID && backpackItem.curStack < backpackItem.maxStack)
                {
                    backpackItem.curStack += stackLeft;
                    if (backpackItem.curStack > backpackItem.maxStack)
                    {
                        stackLeft = backpackItem.curStack - backpackItem.maxStack;
                        item.curStack = stackLeft;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        if (AddItem)
        {
            for (int i = 0; i < _playerInventory.Count; i++)
            {
                if (_playerInventory[i] == null)
                {
                    _playerInventory[i] = item;
                    return true;
                }
            }
        }
        return false;
    }

    public Dictionary<CombatStatsType, float> GetEquippedItemStats()
    {
        Dictionary<CombatStatsType, float> stats = new Dictionary<CombatStatsType, float>();
        int enumCount = typeof(CombatStatsType).GetEnumCount();
        for (int i = 0; i< enumCount; i++)
        {
            CombatStatsType statType = (CombatStatsType)i;
            stats[statType] = 0;
        }
        foreach(ItemData item in _playerEquipments)
        {
            if (item!= null)
            {
                foreach(ItemAttribute attribute in item.itemAttributes)
                {
                    stats[attribute.AtrbName.CastToCombatStatsType()] += attribute.AtrbValue;
                }
            }
        }
        return stats;
    }

    public void SetItemInPlayerBackpack(ItemData item, int index, StoredItemPlaceType placeType)
    {
        switch (placeType)
        {
            case StoredItemPlaceType.PlayerBackpack:
                _playerInventory[index] = item;
                break;
            case StoredItemPlaceType.PlayerEquipment:
                _playerEquipments[index] = item;
                break;
            case StoredItemPlaceType.PlayerHotbar:
                _playerHotbar[index] = item;
                break;
        }
    }

    public void SetPlayerBackpackItem(ItemData item, int index)
    {
        _playerInventory[index] = item;
    }
    public void SetPlayerEquippedItem(ItemData item, int index)
    {
        _playerEquipments[index] = item;
        OnStatsChanged?.Invoke();
    }
    public void SetPlayerHotbarItem(ItemData item, int index)
    {
        _playerHotbar[index] = item;
    }

    public void SetPlayerBackPackRange(List<ItemData> items)
    {
        _playerInventory = items;
    }

    public void SetPlayerEquippedItemsRange(List<ItemData> items)
    {
        if (items.Count > 8) return;
        for(int i = 0; i < items.Count; i++)
        {
            _playerEquipments[i] = items[i];
        }
        OnStatsChanged?.Invoke();
    }

    public void SetPlayerHotbarItemsRange(List<ItemData> items)
    {
        if (items.Count > 6) return;
        for (int i = 0; i < items.Count; i++)
        {
            _playerHotbar[i] = items[i];
        }
    }

    public List<ItemData> GetPlayerBackpackItems() => _playerInventory;
    public List<ItemData> GetPlayerEquippedItems() => _playerEquipments.ToList();
    public List<ItemData> GetPlayerHotbarItems() => _playerHotbar.ToList();

    //public List<ItemData> GetClonePlayerInventory() => CloneItemList(_playerInventory);
    //public List<ItemData> GetClonePlayerEquipment() => CloneItemList(GetPlayerEquippedItems());
    //public List<ItemData> GetClonePlayerHotbar() => CloneItemList(GetPlayerHotbarItems());

    List<ItemData> CloneItemList(List<ItemData> sourceList)
    {
        List<ItemData> cloneList = new List<ItemData>(sourceList);
        for (int i = 0; i < _playerInventory.Count; i++)
        {
            ItemData item = _playerInventory[i];
            if (item != null)
                cloneList.Add(item.Clone());
            else
                cloneList.Add(null);
        }
        return cloneList;
    }
}

public enum StoredItemPlaceType
{
    NONE, PlayerBackpack, PlayerEquipment, PlayerHotbar, Box
}
