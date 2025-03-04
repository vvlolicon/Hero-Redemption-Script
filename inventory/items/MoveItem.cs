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
    public GeneralStatsObj playerStats;
    [HideInInspector]public Item curSlotItem;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObject = eventData.pointerDrag;
        DraggableItem dropDragItem = dropObject.GetComponent<DraggableItem>();
        ItemType dropItemType = dropObject.GetComponent<ItemDetail>().item.itemType;
        GameObject dropItemOriginSlot = dropDragItem.parentAfterDrag.gameObject;
        GameObject dropItemOriginWindow = dropDragItem.parentAfterDrag.parent.gameObject;
        GameObject targetSlotWindow = transform.parent.gameObject;
        Item droppedItem = dropObject.GetComponent<ItemDetail>().item;

        //Debug.Log("##########Test Variables##########");
        //Debug.Log("targetItemSlot = " + targetItemSlot.name);
        //Debug.Log("targetItemInterface = " + targetItemInterface.name + " / tag: " + targetItemInterface.tag);
        //Debug.Log("targetSlotInterface = " + targetSlotInterface.name + " / tag: " + targetSlotInterface.tag);
        Debug.Log("transform.childCount: " + transform.childCount);
        if (transform.childCount > 0) // has item in target dropped window
        {
            //Debug.Log("target slot has item inside");
            //Debug.Log("is targetSlotInterface = targetItemInterface: " + targetSlotInterface.CompareTag(targetItemInterface.tag));
            //Debug.Log("dragItemInSlot.parentAfterDrag = " + dragItemInSlot.parentAfterDrag.name);
            Item targetItem = transform.GetChild(0).GetComponent<ItemDetail>().item;
            DraggableItem targetDragItem = transform.GetChild(0).GetComponent<DraggableItem>();
            if (targetSlotWindow.CompareTag(dropItemOriginWindow.tag)&& !(targetSlotWindow.CompareTag("Player_Equipment"))&& !(dropItemOriginWindow.CompareTag("Player_Equipment")))
            {
                Debug.Log("dropping item from inventory to inventory");
                exchangeItem(targetDragItem, dropDragItem);
            }
            else if (targetSlotWindow.CompareTag("Player_Equipment"))
            {
                // check drag item is drag from equipment slot
                if (dropItemOriginWindow.CompareTag("Player_Inventory"))
                {
                    //Debug.Log("dropping item from inventory to equipment");
                    //if target exchange item type is equal to drag item type, exchange the position of target item with drag item 
                    if (compareItemType(targetItem.itemType,ThisSlotType))
                    {
                        //Debug.Log("droppedItem: " + droppedItem.itemName + " / targetItem: " + targetItem.itemName);
                        UpdateItemAttribute(droppedItem);
                        exchangeItem(targetDragItem, dropDragItem);
                    } else
                    {
                        // find a empty place in inventory to put the item
                        for (int i = 0; i < targetSlotWindow.transform.childCount; i++)
                        {
                            Transform slot = targetSlotWindow.transform.GetChild(i);
                            if (slot.childCount == 0)
                            {
                                slot.GetComponent<MoveItem>().dropItem(dropDragItem);
                                dropItemOriginSlot.GetComponent<MoveItem>().removeItemAttribute();
                                break;
                            }
                        }
                    }
                }
                
            }
            else if (targetSlotWindow.CompareTag("Player_Equipment"))
            {
                if (compareItemType(dropItemType, ThisSlotType))
                {
                    UpdateItemAttribute(targetItem);
                    exchangeItem(targetDragItem, dropDragItem);
                }
            }
            else if (targetSlotWindow.CompareTag("Player_HotbarItem") && dropItemType == ItemType.Consumable)
            {
                exchangeItem(targetDragItem, dropDragItem);
            }
        }
        else // does not have item in target dropped window
        {
            if (targetSlotWindow.CompareTag("Player_Inventory"))
            {
                dropItem(dropDragItem);
                if (dropItemOriginWindow.CompareTag("Player_Equipment"))
                {
                    dropItemOriginSlot.GetComponent<MoveItem>().removeItemAttribute();
                }
            }
            else if (targetSlotWindow.CompareTag("Player_Equipment") && compareItemType(dropItemType, ThisSlotType))
            {
                UpdateItemAttribute(droppedItem);
                dropItem(dropDragItem);
            }
            else if (targetSlotWindow.CompareTag("Player_HotbarItem") && dropItemType == ItemType.Consumable)
            {
                dropItem(dropDragItem);
            }

        }
        //Debug.Log("##########Test Variables End##########");
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
        //Debug.Log("Register item attribute: " + curSlotItem.name);

        foreach (ItemAttribute itemAttr in curSlotItem.itemAttributes)
        {
            ChangePlayerStats(itemAttr, 1);
        }
    }

    public void removeItemAttribute()
    {
        if (curSlotItem != null)
        {
            //Debug.Log("remove item attribute: " + curSlotItem.name);
            foreach (ItemAttribute itemAttr in curSlotItem.itemAttributes)
            {
                ChangePlayerStats(itemAttr, -1);
            }
            curSlotItem = null;
        }
        else
        {
            //Debug.Log("null item");
        }
    }

    public void ChangePlayerStats(ItemAttribute itemAttr, int multiplier)
    {
        float valueChanged = itemAttr.AtrbValue * multiplier;
        switch (itemAttr.AtrbName)
        {
            case ItemAttributeName.MaxHP:
                if (playerStats.MaxHP == playerStats.HP)
                {
                    playerStats.HP += valueChanged;
                }
                playerStats.MaxHP += valueChanged;
                break;
            case ItemAttributeName.MaxMP:
                if (playerStats.MaxMP == playerStats.MP)
                {
                    playerStats.MP += valueChanged;
                }
                playerStats.MaxMP += valueChanged;
                break;
            case ItemAttributeName.ATK:
                playerStats.ATK += valueChanged;
                break;
            case ItemAttributeName.AtkTime:
                playerStats.AttackTime += valueChanged;
                break;
            case ItemAttributeName.DEF:
                playerStats.DEF += valueChanged;
                break;
            case ItemAttributeName.SPEED:
                playerStats.SPEED += valueChanged;
                break;
            case ItemAttributeName.CritChance:
                playerStats.CritChance += valueChanged;
                break;
            case ItemAttributeName.CritChanRdc:
                playerStats.CritChanRdc += valueChanged;
                break;
            case ItemAttributeName.CritDmgMult:
                playerStats.CritDmgMult += valueChanged;
                break;
            case ItemAttributeName.CritDmgResis:
                playerStats.CritDmgResis += valueChanged;
                break;
            case ItemAttributeName.DmgReduce:
                playerStats.DmgReduce += valueChanged;
                break;
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
