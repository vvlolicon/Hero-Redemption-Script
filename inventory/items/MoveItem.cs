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
        GameObject targetItemSlot = dropDragItem.parentAfterDrag.gameObject;
        GameObject targetItemInterface = dropDragItem.parentAfterDrag.parent.gameObject;
        GameObject targetSlotInterface = transform.parent.gameObject;
        Item droppedItem = dropObject.GetComponent<ItemDetail>().item;

        //Debug.Log("##########Test Variables##########");
        //Debug.Log("targetItemSlot = " + targetItemSlot.name);
        //Debug.Log("targetItemInterface = " + targetItemInterface.name + " / tag: " + targetItemInterface.tag);
        //Debug.Log("targetSlotInterface = " + targetSlotInterface.name + " / tag: " + targetSlotInterface.tag);
        Debug.Log("transform.childCount: " + transform.childCount);
        if (transform.childCount > 0)
        {
            //Debug.Log("target slot has item inside");
            //Debug.Log("is targetSlotInterface = targetItemInterface: " + targetSlotInterface.CompareTag(targetItemInterface.tag));
            //Debug.Log("dragItemInSlot.parentAfterDrag = " + dragItemInSlot.parentAfterDrag.name);
            Item targetItem = transform.GetChild(0).GetComponent<ItemDetail>().item;
            DraggableItem targetDragItem = transform.GetChild(0).GetComponent<DraggableItem>();
            if (targetSlotInterface.CompareTag(targetItemInterface.tag)&& !(targetSlotInterface.CompareTag("player_equipment"))&& !(targetItemInterface.CompareTag("player_equipment")))
            {
                Debug.Log("dropping item from inventory to inventory");
                exchangeItem(targetDragItem, dropDragItem);
            }
            else if (targetSlotInterface.CompareTag("player_equipment"))
            {
                // check drag item is drag from equipment slot
                if (targetItemInterface.CompareTag("player_inventory"))
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
                        for (int i = 0; i < targetSlotInterface.transform.childCount; i++)
                        {
                            Transform slot = targetSlotInterface.transform.GetChild(i);
                            if (slot.childCount == 0)
                            {
                                slot.GetComponent<MoveItem>().dropItem(dropDragItem);
                                targetItemSlot.GetComponent<MoveItem>().removeItemAttribute();
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
                if (targetItemInterface.CompareTag("player_equipment"))
                {
                    targetItemSlot.GetComponent<MoveItem>().removeItemAttribute();
                }
            }
            else if (targetSlotInterface.CompareTag("player_equipment") && compareItemType(dropItemType, ThisSlotType))
            {
                UpdateItemAttribute(droppedItem);
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
        switch (itemAttr.AtrbName)
        {
            case ItemAttributeName.MaxHP:
                playerStats.MaxHP += itemAttr.AtrbValue * multiplier;
                // also changes the hp when changing MaxHP, and makesure at least 1 HP left
                playerStats.HP = Mathf.Max(playerStats.HP + itemAttr.AtrbValue * multiplier, 1); 
                break;
            case ItemAttributeName.MaxMP:
                playerStats.MaxMP += itemAttr.AtrbValue * multiplier;
                // also change MP when max MP changes, and makesure at least 0 MP
                playerStats.MP = Mathf.Max(playerStats.MP + itemAttr.AtrbValue * multiplier, 0); 
                break;
            case ItemAttributeName.ATK:
                playerStats.ATK += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.AtkTime:
                playerStats.AttackTime += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.DEF:
                playerStats.DEF += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.SPEED:
                playerStats.SPEED += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.CritChance:
                playerStats.CritChance += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.CritChanRdc:
                playerStats.CritChanRdc += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.CritDmgMult:
                playerStats.CritDmgMult += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.CritDmgResis:
                playerStats.CritDmgResis += itemAttr.AtrbValue * multiplier;
                break;
            case ItemAttributeName.DmgReduce:
                playerStats.DmgReduce += itemAttr.AtrbValue * multiplier;
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
