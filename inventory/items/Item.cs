using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;

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

