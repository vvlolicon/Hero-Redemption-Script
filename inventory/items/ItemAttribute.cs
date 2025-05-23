﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ItemAttribute
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
    CritChance, CritChanRdc, CritDmgMult, CritDmgResis, DmgReduce, MP_Regen,
    // these attributes should only use on pickup item
    Money, EXP, LEVEL
}
[System.Serializable]
public class ItemDropChance
{
    public float dropWeight;
    public Item dropItem;
    [HideInInspector]public float dropChance;
    public ItemDropChance(float dropWeight, Item dropItem)
    {
        this.dropWeight = dropWeight;
        this.dropItem = dropItem;
    }
}
