using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    GameObject equipmentItemContainer { get { return UI_Controller.GetInventoryItemContainer(UI_Window.EquipmentUI); } }
    GameObject inventory_ui { get { return UI_Controller.GetInventoryItemContainer(UI_Window.InventoryUI); } }
    GameObject BoxInventoryContainer { get { return UI_Controller.GetInventoryItemContainer(UI_Window.BoxInventoryUI); } }
    // if using this method, it will conflict with drag event, so i have to use onPointerUp
    // but idk why, this method must be there or onPointerUp method not works
    // and don't put anything in this method, even comment, or will break the function
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        // check if the item is in any slot (drag func will change the parent it to somewhere)
        if (transform.parent.parent != null)
        {
            DraggableItem clickDragItem = gameObject.GetComponent<DraggableItem>();
            Item clickItem = gameObject.GetComponent<ItemDetail>().item;
            
            //Debug.Log("click Item: " + clickItem.itemName);
            GameObject item_at_interface = transform.parent.parent.gameObject;
            GameObject itemSlot = transform.parent.gameObject;
            //Debug.Log("item_at_interface: " + item_at_interface.name);
            //Debug.Log("equipment_ui: " + equipment_ui.name);
            //Debug.Log("inventory ui: " + inventory_ui.name);
            // if item is a consumable item(e.g. potion) and item is in player inventory or hot bar, use it 
            if (clickItem.itemType == ItemType.Consumable)
            {
                if (item_at_interface.CompareTag("Player_Inventory") || item_at_interface.CompareTag("Player_HotbarItem"))
                {
                    GetComponent<ConsumableItem>().ConsumeItem();
                }

            }
            // if equipment ui is opened and item is in player inventory, try to put item into equipment slot
            else
            {
                if (item_at_interface.CompareTag("Player_Inventory"))
                {
                    if (equipmentItemContainer.activeInHierarchy)
                    {
                        //find slots and compare item type to equipment slot type
                        for (int i = 0; i < equipmentItemContainer.transform.childCount; i++)
                        {
                            Transform equipment_slot = equipmentItemContainer.transform.GetChild(i);
                            var equipmentSlotType = equipment_slot.GetComponent<MoveItem>().ThisSlotType;
                            //Debug.Log($"equipmentSlotType: {equipmentSlotType}, clickItem type: {clickItem.itemType}");
                            if (clickItem.itemType.CompareItemType(equipmentSlotType))
                            {
                                // if equipment slot don't contain any item, put it
                                if (equipment_slot.childCount == 0)
                                {
                                    equipment_slot.GetComponent<MoveItem>().UpdateItemAttribute(clickItem);
                                    //Debug.Log("find slot: " + equipment_slot.gameObject.name);
                                    clickDragItem.parentAfterDrag = equipment_slot;
                                    transform.SetParent(equipment_slot);
                                    break;
                                }
                            }
                        }
                    }
                    else if (BoxInventoryContainer.activeInHierarchy)
                    {
                        Debug.Log("Try to move object from player inventory to box");
                    }
                }

                else if (item_at_interface.CompareTag("Player_Equipment") && inventory_ui.activeInHierarchy)
                {// if item is in player equipment, try to put it back to inventory
                    MoveItemTo(itemSlot.transform, inventory_ui.transform, clickDragItem);
                }
                else if (item_at_interface.CompareTag("Box_Inventory") && inventory_ui.activeInHierarchy)
                {// if item is in Box, try to put it back to inventory
                    item_at_interface.GetComponent<InventorySlotManager>().InvokeEvent(
                        itemSlot.transform.GetIndexOfChild(), clickItem, true);
                    MoveItemTo(itemSlot.transform, inventory_ui.transform, clickDragItem);
                }
            }
        }
    }

    public void MoveItemTo(Transform originSlot, Transform targetInventory, DraggableItem clickDragItem)
    {
        for (int i = 0; i < targetInventory.childCount; i++)
        {
            Transform inventory_slot = targetInventory.GetChild(i);
            // if slot don't contain any item, put it
            if (inventory_slot.childCount == 0)
            {
                //Debug.Log("remove item at slot: " + transform.parent.gameObject.name);
                //Debug.Log("remove item name: " + transform.parent.GetComponent<MoveItem>().curSlotItem.itemName);
                if (originSlot.CompareTag("Player_Equipment") &&
                    !targetInventory.CompareTag("Player_Equipment"))
                {
                    originSlot.GetComponent<MoveItem>().removeItemAttribute();
                }
                clickDragItem.parentAfterDrag = inventory_slot;
                transform.SetParent(inventory_slot);
                break;
            }
        }
    }
}
