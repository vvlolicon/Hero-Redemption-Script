using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{
    public ItemAttributeName AtrbName;
    public float AtrbValue;
    public ItemAttribute(ItemAttributeName attributeName, float attributeValue)
    {
        this.AtrbName = attributeName;
        this.AtrbValue = attributeValue;    
    }

}
public enum ItemAttributeName { 
    MaxHP, HP, MaxMP, MP, ATK, AtkTime, DEF, SPEED, 
    CritChance, CritChanRdc, CritDmgMult, CritDmgResis, DmgReduce,
    // these attributes should only use on pickup item
    Money, EXP
}
[System.Serializable]
public class ItemDropChance
{
    public float dropWeight;
    public Item dropItem;
    public float dropChance { get; set; }
    public ItemDropChance(float dropWeight, Item dropItem)
    {
        this.dropWeight = dropWeight;
        this.dropItem = dropItem;
    }
}
