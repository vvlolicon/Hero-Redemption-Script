using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Inventory/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;                                     //itemName of the item
    public ItemType itemType;
    public int itemID;                                          //itemID of the item
    public string itemDesc;                                     //itemDesc of the item
    public Sprite itemIcon;                                     //itemIcon of the item
    public GameObject itemModel;                                //itemModel of the item
    public int itemValue = 1;                                   //itemValue is at start 1
    //public ItemType itemType;                                   //itemType of the Item
    public float itemWeight;                                    //itemWeight of the item
    public int maxStack = 1;
    public int indexItemInList = 999;    
    public int rarity;

    [SerializeField]
    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();    
    
    public Item(){}

    public Item(string name, int id, ItemType type, string desc, Sprite icon, GameObject model, int maxStack, string sendmessagetext, List<ItemAttribute> itemAttributes)                 //function to create a instance of the Item
    {
        itemName = name;
        itemType = type;
        itemID = id;
        itemDesc = desc;
        itemIcon = icon;
        itemModel = model;
        this.maxStack = maxStack;
        this.itemAttributes = itemAttributes;
    }

    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();        
    }   
}


