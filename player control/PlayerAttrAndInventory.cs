using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Functions/Player Attribute And Inventory")]
public class PlayerAttrAndInventory : ScriptableObject
{
    public float EXP = 0;
    public float Money = 0;

    [SerializeField]
    public List<Item> Items = new List<Item>();
}
