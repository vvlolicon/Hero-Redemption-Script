using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Items/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;                                     //itemName of the item
    public ItemType itemType;
    public ConsumableItemType consumableItemType;
    public int itemID;                                          //itemID of the item
    public string itemDesc;                                     //itemDesc of the item
    public Sprite itemIcon;                                     //itemIcon of the item
    public GameObject itemModel;                                //itemModel of the item
    public int itemValue = 1;                                   //itemValue is at start 1
    public float itemWeight;                                    //itemWeight of the item
    public int maxStack = 1;
    public int setStack = 1;    
    public int rarity;
    
    [SerializeField]
    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();

    public int curStack { get; set; }

    public Item(){}

    //function to create a instance of the Item
    public Item(string name, int id, ItemType type, string desc, Sprite icon, GameObject model, int maxStack, int setStack, List<ItemAttribute> itemAttributes)                 
    {
        itemName = name;
        itemType = type;
        itemID = id;
        itemDesc = desc;
        itemIcon = icon;
        itemModel = model;
        this.maxStack = maxStack;
        this.setStack = setStack;
        this.itemAttributes = itemAttributes;
        curStack = setStack;
    }

    public ItemData GetItemDataClone()
    {
        return new ItemData(this);
    }

    private void OnEnable()
    {
        curStack = setStack;
    }

    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();        
    }   
}

public enum ItemType
{
    Any = 0,
    Helmet = 1,
    Armor = 2,
    Gloves = 3,
    Shoe = 4,
    Necklace = 5,
    Ring = 6,
    Weapon = 7,
    Consumable = 8,
    Weapon_Melee,
    Weapon_Range,
    Backpack,
    Ammo
}

public enum ConsumableItemType
{
    None, InstantPotion, TimedPotion, Scroll
}
[System.Serializable]
public class ItemData
{
    public string itemName;                                     
    public ItemType itemType { get { return prototype.itemType; } }
    public ConsumableItemType consumableItemType { get { return prototype.consumableItemType; } }
    public int itemID;                                          
    public string itemDesc;
    public int itemValue = 1; 
    public float itemWeight;
    public int maxStack = 1;
    public int curStack = 1;
    public int rarity; 
    public List<ItemAttribute> itemAttributes = new();

    // resouces that cannot be serialized
    [System.NonSerialized] public Sprite itemIcon;
    [System.NonSerialized] public GameObject itemModel;
    [System.NonSerialized] public Item prototype;

    public ItemData(Item prototype)
    {
        this.prototype = prototype;
        itemID = prototype.itemID;
        itemName = prototype.itemName;
        itemDesc = prototype.itemDesc;
        itemIcon = prototype.itemIcon;
        itemValue = prototype.itemValue;
        maxStack = prototype.maxStack;
        curStack = prototype.curStack;
        itemAttributes = prototype.itemAttributes;
    }

    #region ICloneable Members

    public ItemData Clone()
    {
        return (ItemData) this.MemberwiseClone();
    }

    #endregion

    public ItemData()
    {
        itemID = -1;
    }

}

