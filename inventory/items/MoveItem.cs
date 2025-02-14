using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class MoveItem : MonoBehaviour, IDropHandler
{
    public ItemType ThisSlotType;
    public PlayerStats playerStats;
    [HideInInspector]public Item curSlotItem;

    void Start()
    {
        //GameObject exampleItem = Resources.Load<GameObject>("prefab/item");
        //GameObject newItem = Instantiate(exampleItem);
        //newItem.transform.SetParent(this.transform);
        //UpdateItemAttribute(exampleItem.GetComponent<ItemDetail>().item);
        //Destroy(newItem);
        //Debug.Log("curSlotItem = " + curSlotItem.itemName);
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObject = eventData.pointerDrag;
        DraggableItem dropDragItem = dropObject.GetComponent<DraggableItem>();
        ItemType dropItemType = dropObject.GetComponent<ItemDetail>().item.itemType;
        GameObject originItemSlot = dropDragItem.parentAfterDrag.gameObject;
        GameObject originItemInterface = dropDragItem.parentAfterDrag.parent.gameObject;
        GameObject targetSlotInterface = transform.parent.gameObject;
        Item droppedItem = dropObject.GetComponent<ItemDetail>().item;

        Debug.Log("##########Test Variables##########");
        Debug.Log("originItemSlot = " + originItemSlot.name);
        Debug.Log("originItemInterface = " + originItemInterface.name + " / tag: " + originItemInterface.tag);
        Debug.Log("targetSlotInterface = " + targetSlotInterface.name + " / tag: " + targetSlotInterface.tag);
        Debug.Log("transform.childCount: " + transform.childCount);
        if (transform.childCount > 0)
        {
            Debug.Log("target slot has item inside");
            Debug.Log("is targetSlotInterface = originItemInterface: " + targetSlotInterface.CompareTag(originItemInterface.tag));
            Item targetItem = transform.GetChild(0).GetComponent<ItemDetail>().item;
            DraggableItem targetDragItem = transform.GetChild(0).GetComponent<DraggableItem>();
            //Debug.Log("dragItemInSlot.parentAfterDrag = " + dragItemInSlot.parentAfterDrag.name);
            if (targetSlotInterface.CompareTag(originItemInterface.tag)&& !(targetSlotInterface.CompareTag("player_equipment"))&& !(originItemInterface.CompareTag("player_equipment")))
            {
                Debug.Log("dropping item from inventory to inventory");
                exchangeItem(targetDragItem, dropDragItem);
            }
            else if (targetSlotInterface.CompareTag("player_equipment"))
            {
                // check drag item is drag from equipment slot
                if (originItemInterface.CompareTag("player_inventory"))
                {
                    Debug.Log("dropping item from inventory to equipment");
                    //if target exchange item type is equal to drag item type, exchange the position of target item with drag item 
                    if (compareItemType(targetItem.itemType,ThisSlotType))
                    {
                        Debug.Log("droppedItem: " + droppedItem.itemName + " / targetItem: " + targetItem.itemName);
                        UpdateItemAttribute(droppedItem);
                        exchangeItem(targetDragItem, dropDragItem);
                    } else
                    {
                        // find a empty place in inventory to put the item
                        for (int i = 0; i < targetSlotInterface.transform.childCount; i++)
                        {
                            Transform slot = targetSlotInterface.transform.GetChild(i);
                            if (slot.childCount == 0)
                            {
                                slot.GetComponent<MoveItem>().dropItem(dropDragItem);
                                originItemSlot.GetComponent<MoveItem>().removeItemAttribute();
                                break;
                            }
                        }
                    }
                }
                
            }
            else if (targetSlotInterface.CompareTag("player_equipment"))
            {
                if (compareItemType(dropItemType, ThisSlotType))
                {
                    UpdateItemAttribute(targetItem);
                    exchangeItem(targetDragItem, dropDragItem);
                }
            }
        }
        else
        {
            if (targetSlotInterface.CompareTag("player_inventory"))
            {
                dropItem(dropDragItem);
                if (originItemInterface.CompareTag("player_equipment"))
                {
                    originItemSlot.GetComponent<MoveItem>().removeItemAttribute();
                }
            }
            else if (targetSlotInterface.CompareTag("player_equipment") && compareItemType(dropItemType, ThisSlotType))
            {
                UpdateItemAttribute(droppedItem);
                dropItem(dropDragItem);
            }
            
        }
        Debug.Log("##########Test Variables End##########");
    }

    private void exchangeItem(DraggableItem itemInSlot, DraggableItem itemDrop)
    {
        itemInSlot.parentAfterDrag = itemDrop.parentAfterDrag;
        itemInSlot.gameObject.transform.SetParent(itemDrop.parentAfterDrag);
        itemDrop.parentAfterDrag = transform;
    }

    public void dropItem(DraggableItem itemDrop)
    {
        itemDrop.parentAfterDrag = transform;
    }

    public void UpdateItemAttribute(Item item)
    {
        if (curSlotItem != null) {
            Debug.Log("curSlotItem: " + curSlotItem.itemName + " / reg item:" + item.itemName);
            if (item != curSlotItem)
            {
                
                registerItemAttribute(item);
            }
        }
        else
        {
            //Debug.Log("null item");
            registerItemAttribute(item);
        }
    }

    private void registerItemAttribute(Item itemDetail)
    {
        removeItemAttribute();
        curSlotItem = itemDetail;
        Debug.Log("Register item attribute: " + curSlotItem.name);

        foreach (ItemAttribute itemAttr in curSlotItem.itemAttributes)
        {
            if (itemAttr.attributeName.Equals("MaxHP"))
                playerStats.maxHP += itemAttr.attributeValue;
            if (itemAttr.attributeName.Equals("MaxMP"))
                playerStats.maxMP += itemAttr.attributeValue;
            if (itemAttr.attributeName.Equals("ATK"))
                playerStats.ATK += itemAttr.attributeValue;
            if (itemAttr.attributeName.Equals("DEF"))
                playerStats.DEF += itemAttr.attributeValue;
            if (itemAttr.attributeName.Equals("SPEED"))
                playerStats.SPEED += itemAttr.attributeValue;
        }
    }

    public void removeItemAttribute()
    {
        if (curSlotItem != null)
        {
            Debug.Log("remove item attribute: " + curSlotItem.name);
            foreach (ItemAttribute itemAttr in curSlotItem.itemAttributes)
            {
                if (itemAttr.attributeName.Equals("MaxHP"))
                {
                    playerStats.maxHP -= itemAttr.attributeValue;
                    Debug.Log("remove MaxHP: " + itemAttr.attributeValue);
                }
                else if (itemAttr.attributeName.Equals("MaxMP"))
                    playerStats.maxMP -= itemAttr.attributeValue;
                else if (itemAttr.attributeName.Equals("ATK"))
                {
                    playerStats.ATK -= itemAttr.attributeValue;
                    Debug.Log("remove atk: " + itemAttr.attributeValue);
                }
                else if (itemAttr.attributeName.Equals("DEF"))
                {
                    playerStats.DEF -= itemAttr.attributeValue;
                    Debug.Log("remove def: " + itemAttr.attributeValue);
                }
                else if (itemAttr.attributeName.Equals("SPEED"))
                    playerStats.SPEED -= itemAttr.attributeValue;
            }
            curSlotItem = null;
        }
        else
        {
            //Debug.Log("null item");
        }
    }

    public static bool compareItemType(ItemType type1, ItemType type2)
    {
        if(type1 == type2)
            return true;
        else if(type1 == ItemType.Weapon || type2 == ItemType.Weapon)
        {
            if (type1 == ItemType.Weapon_Melee || type1 == ItemType.Weapon_Range || type2 == ItemType.Weapon_Melee || type2 == ItemType.Weapon_Range)
                return true;
            else
                return false;
        }
        else
            return false;
    }
}
