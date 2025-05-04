using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Drop Data")]
public class EnemyDropData : ScriptableObject 
{
    //public int EXP_drop;
    public int Money_drop;
    public List<Item> EnsureDropItem = new List<Item>();
    public List<RandomDropTable> RandomDropTable = new List<RandomDropTable>();

    public List<Item> GetDropItems()
    {
        List<Item> dropItems = new List<Item>();
        if(EnsureDropItem.Count > 0)
            dropItems.AddRange(EnsureDropItem);
        if (RandomDropTable.Count > 0)
        {
            foreach (RandomDropTable table in RandomDropTable)
            {
                Item dropItem = table.GetRandomDrop();
                if (dropItem != null)
                {
                    Debug.Log($"Dropped item: {dropItem.itemName}");
                    dropItems.Add(dropItem);
                }
            }
        }
        return dropItems;
    }
}
