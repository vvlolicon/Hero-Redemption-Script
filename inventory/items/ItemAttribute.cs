using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemAttribute
{

    public ItemAttributeName AtrbName;
    public float AtrbValue;
    public ItemAttribute(ItemAttributeName attributeName, int attributeValue)
    {
        this.AtrbName = attributeName;
        this.AtrbValue = attributeValue;
    }

}
public enum ItemAttributeName { MaxHP, HP, MaxMP, MP, ATK, AtkTime, DEF, SPEED, CritChance, CritMult, CritResis, DmgReduce }

