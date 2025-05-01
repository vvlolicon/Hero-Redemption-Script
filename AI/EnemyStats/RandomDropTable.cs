using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Random Drop Table")]
public class RandomDropTable : ScriptableObject
{
    [SerializeField]
    public List<ItemDropChance> ItemDropList = new List<ItemDropChance>();

    private void Awake()
    {
        if(ItemDropList.Count > 0)
        {
            float sumWeight = 0;
            foreach (ItemDropChance chance in ItemDropList)
            {
                sumWeight += chance.dropWeight;
            }
            foreach (ItemDropChance chance in ItemDropList)
            {
                chance.dropChance = chance.dropWeight / sumWeight;
            }
        }
    }

    public Item GetRandomDrop()
    {
        if (ItemDropList.Count == 0) return null;
        float random = Random.value;
        float sum = 0;
        foreach (ItemDropChance chance in ItemDropList)
        {
            sum += chance.dropChance;
            if (random <= sum)
            {
                return chance.dropItem;
            }
        }
        return ItemDropList[ItemDropList.Count - 1].dropItem;
    }
}


