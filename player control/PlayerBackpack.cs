using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] List<Item> _playerOwnedItems;
    public int PlayerBackpackSize = 20;
    [SerializeField] Item[] _playerEquippedItems = new Item[8];
    [SerializeField] Item[] _playerHotbarItems = new Item[6];
    [HideInInspector] public int PlayerLevel = 1;
    [HideInInspector] public int PlayerOwnedMoney = 0;
    //public int PlayerExp;

    public event CombatBuffHandler.OnStatsChangedDelegate OnStatsChanged;

    private void Awake()
    {
        List<Item> newList = new List<Item>(PlayerBackpackSize);
        for (int i = 0; i < PlayerBackpackSize; i++)
        {
            if (i < _playerOwnedItems.Count)
                newList.Add(_playerOwnedItems[i]);
            else
                newList.Add(null);
        }
        _playerOwnedItems = newList;
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
                if (!AddItemToPlayerBackpack(item))
                {
                    Debug.Log("Backpack is full, dropping " + item.itemName + " on the ground");
                }
            }
        }
    }

    public bool AddItemToPlayerBackpack(Item item)
    {
        for (int i = 0; i < _playerOwnedItems.Count; i++)
        {
            if (_playerOwnedItems[i] == null)
            {
                _playerOwnedItems[i] = item;
                return true;
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
        foreach(Item item in _playerEquippedItems)
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

    public void SetItemInPlayerBackpack(Item item, int index, StoredItemPlaceType placeType)
    {
        switch (placeType)
        {
            case StoredItemPlaceType.PlayerBackpack:
                _playerOwnedItems[index] = item;
                break;
            case StoredItemPlaceType.PlayerEquipment:
                _playerEquippedItems[index] = item;
                break;
            case StoredItemPlaceType.PlayerHotbar:
                _playerHotbarItems[index] = item;
                break;
        }
    }

    public void SetPlayerBackpackItem(Item item, int index)
    {
        _playerOwnedItems[index] = item;
    }
    public void SetPlayerEquippedItem(Item item, int index)
    {
        _playerEquippedItems[index] = item;
        OnStatsChanged?.Invoke();
    }
    public void SetPlayerHotbarItem(Item item, int index)
    {
        _playerEquippedItems[index] = item;
    }

    public List<Item> GetPlayerBackpackItems() => _playerOwnedItems;
    public List<Item> GetPlayerEquippedItems() => _playerEquippedItems.ToList();
    public List<Item> GetPlayerHotbarItems() => _playerHotbarItems.ToList();
}

public enum StoredItemPlaceType
{
    NONE, PlayerBackpack, PlayerEquipment, PlayerHotbar, Box
}
