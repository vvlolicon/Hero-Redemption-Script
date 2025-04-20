using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Drop Table")]
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
}


