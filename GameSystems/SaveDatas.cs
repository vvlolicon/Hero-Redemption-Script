using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.SaveSystem.PlayerData;

namespace Assets.SaveSystem
{
    [System.Serializable]
    public class PlayerData
    {
        public int playerLevel;
        public int playerMoney;
        public Vector3 playerPos;
        public List<ItemData> playerInventory;
        public List<ItemData> playerEquipment;
        public List<ItemData> playerHotbar;

        public string curStageID;

        [System.Serializable]
        public struct SerializableDictionary
        {
            public CombatStatsType key;
            public float value;
        }

        public List<SerializableDictionary> serializedCombatStats = new();
        public List<SerializableDictionary> serializedExtraStats = new();

        [System.NonSerialized]
        public Dictionary<CombatStatsType, float> combatStats;
        [System.NonSerialized]
        public Dictionary<CombatStatsType, float> extraStats;

        public void SerializeData()
        {
            serializedCombatStats.SetCombatStats(combatStats);
            serializedExtraStats.SetCombatStats(extraStats);
        }

        public void DeserializeData()
        {
            playerInventory.RebuildItemList();
            playerEquipment.RebuildItemList();
            playerHotbar.RebuildItemList();
            if(combatStats == null && serializedCombatStats!= null)
                combatStats = serializedCombatStats.DeserializeStats();
            if(extraStats == null && serializedExtraStats!= null)
                extraStats = serializedExtraStats.DeserializeStats();
        }
    }

    public static class SerializeDataMethods
    {
        public static void SetCombatStats(
            this List<SerializableDictionary> data, 
            Dictionary<CombatStatsType, float> source)
        {
            data.Clear();
            int dictCount = typeof(CombatStatsType).GetEnumCount();
            for (int i = 0; i < dictCount; i++)
            {
                CombatStatsType key = (CombatStatsType)i;
                if (source.ContainsKey(key))
                {
                    data.Add(new SerializableDictionary { key = key, value = source[key] });
                }
            }
        }

        public static Dictionary<CombatStatsType, float> DeserializeStats(this List<SerializableDictionary> data)
        {
            var dict = new Dictionary<CombatStatsType, float>();
            for (int i = 0; i < data.Count; i++)
            {
                CombatStatsType key = (CombatStatsType)i;
                dict[key] = data[i].value;
            }
            return dict;
        }

        public static void SerializeItemList(this List<ItemData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    list[i] = new ItemData();
            }
        }

        public static void RebuildItemList(this List<ItemData> list)
        {
            Item[] itemPrototypes = Resources.LoadAll<Item>("");
            for (int i = 0; i < list.Count; i++)
            {
                Item matchItem = locateItemPrototype(list[i]);
                if (matchItem != null)
                {
                    ItemData newItem = matchItem.GetItemDataClone();
                    newItem.curStack = list[i].curStack;
                    list[i] = newItem;
                }
                else
                {
                    list[i] = null;
                }
            }

            Item locateItemPrototype(ItemData data)
            {
                if (data.itemID < 0) return null;
                foreach (Item item in itemPrototypes)
                {
                    if (item.itemID == data.itemID)
                    {
                        return item;
                    }
                }
                return null;
            }
        }
    }
}