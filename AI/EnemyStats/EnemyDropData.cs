using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Drop Data")]
public class EnemyDropData : ScriptableObject 
{
    public int EXP_drop;
    public int Money_drop;
    public List<Item> EnsureDropItem = new List<Item>();
    public List<RandomDropTable> RandomDropTable = new List<RandomDropTable>();
}
