using UnityEngine;
using UnityEngine.EventSystems;

public class MoveItem : MonoBehaviour, IDropHandler
{
    public ItemType ThisSlotType;
    PlayerStateExecutor Player { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    PlayerBackpack PlayerBackpack { get { return PlayerCompManager.TryGetPlayerComp<PlayerBackpack>(); } }
    PlayerStateExecutor PlayerStatsHandler { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    GeneralCombatStats PlayerStats { get { return Player.PlayerCombatStats; } }
    [HideInInspector]public ItemData curSlotItem;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropObject = eventData.pointerDrag;
        DraggableItem dropDragItem = dropObject.GetComponent<DraggableItem>();
        ItemType dropItemType = dropObject.GetComponent<ItemDetail>().item.itemType;

        GameObject dropItemOriginSlot = dropDragItem.parentAfterDrag.gameObject;
        GameObject dropItemOriginWindow = dropDragItem.parentAfterDrag.parent.gameObject;
        StoredItemPlaceType originWindowPlaceType = dropItemOriginWindow.GetStoredPlaceType();

        GameObject targetSlotWindow = transform.parent.gameObject;
        GameObject targetSlot = gameObject;
        StoredItemPlaceType targetWindowPlaceType = targetSlotWindow.GetStoredPlaceType();

        int targetSlotIndex = transform.GetIndexInParent();
        int originSlotIndex = dropItemOriginSlot.transform.GetIndexInParent();
        ItemData droppedItem = dropObject.GetComponent<ItemDetail>().item;

        ItemData targetItem;
        DraggableItem targetDragItem;
        //Debug.Log("##########Test Variables##########");
        //Debug.Log("target slot has item inside");
        //Debug.Log("dropItemOriginWindow" + dropItemOriginWindow.name);
        //Debug.Log("dropItemOriginSlot = " + dropItemOriginSlot.name);
        //Debug.Log("targetSlotWindow = " + targetSlotWindow.name);
        //Debug.Log("transform.childCount: " + transform.childCount);
        if (transform.childCount > 0) // has item in target dropped window
        {
            targetItem = transform.GetChild(0).GetComponent<ItemDetail>().item;
            targetDragItem = transform.GetChild(0).GetComponent<DraggableItem>();
            if(droppedItem.itemID == targetItem.itemID && 
               droppedItem.itemType == ItemType.Consumable &&
               targetItem.maxStack - targetItem.curStack > droppedItem.curStack)
            {// if target item is the same type and has space for more stack, add stack to target item
                targetItem.curStack += droppedItem.curStack;
                targetDragItem.GetComponent<ItemDetail>().UpdateItem();
                Destroy(dropDragItem.gameObject);
                // delete item data in origin slot script
                if(originWindowPlaceType == StoredItemPlaceType.Box)
                {
                    dropItemOriginWindow.GetComponent<InventorySlotManager>().InvokeEvent(
                        dropItemOriginSlot.transform.GetIndexInParent(), droppedItem, true);
                }
                else
                {
                    PlayerBackpack.SetItemInPlayerBackpack(null, originSlotIndex, originWindowPlaceType);
                }
                return;
            }
            if (targetWindowPlaceType == StoredItemPlaceType.PlayerEquipment)
            {
                if (originWindowPlaceType == StoredItemPlaceType.PlayerInventory)
                { // item is drag from player inventory slot to equipment slot

                    //if target exchange item type is equal to drag item type,
                    //exchange the position of target item with drag item 
                    if (dropItemType.CompareItemType(ThisSlotType))
                    {// if item is the type that the equipment slot restricted
                        //Debug.Log("droppedItem: " + droppedItem.itemName + " / targetItem: " + targetItem.itemName);
                        UpdateItemAttribute(droppedItem);
                        exchangeItem();
                    }
                }
            }
            else if (targetWindowPlaceType == StoredItemPlaceType.PlayerInventory)
            {
                if (originWindowPlaceType == StoredItemPlaceType.PlayerEquipment)
                {// item is drag from equipment slot to player inventory slot
                    if (targetItem.itemType.CompareItemType(ThisSlotType))
                    { // if item is the type that the equipment slot restricted
                        //Debug.Log("droppedItem: " + droppedItem.itemName + " / targetItem: " + targetItem.itemName);
                        UpdateItemAttribute(droppedItem);
                        exchangeItem();
                    }
                    else
                    { // target item does not match the equipment slot restriction
                      // move dropped item to empty space of player inventory
                        PlayerBackpack.SetPlayerEquippedItem(null, originSlotIndex);
                        dropObject.GetComponent<ClickItem>().MoveItemTo(
                            dropItemOriginSlot.transform, 
                            targetSlotWindow.transform, 
                            dropDragItem);
                    }
                }
            }
            else if (targetWindowPlaceType == StoredItemPlaceType.PlayerHotbar &&
                dropItemType == ItemType.Consumable)
            {
                exchangeItem();
            }
            else
            {
                exchangeItem();
            }
        }
        else // does not have item in target dropped window
        {
            switch (targetWindowPlaceType)
            {
                case StoredItemPlaceType.PlayerEquipment:
                    if (dropItemType.CompareItemType(ThisSlotType))
                    {
                        UpdateItemAttribute(droppedItem);
                        dropItem();
                    }
                    break;
                case StoredItemPlaceType.PlayerHotbar:
                    if (dropItemType == ItemType.Consumable)
                    {
                        dropItem();
                    }
                    break;
                default:
                    dropItem();
                    break;
            }
            if(originWindowPlaceType == StoredItemPlaceType.PlayerEquipment)
            {
                dropItemOriginSlot.GetComponent<MoveItem>().RemoveItemAttribute();
            }
        }

        if (originWindowPlaceType == StoredItemPlaceType.Box && 
            targetWindowPlaceType != StoredItemPlaceType.Box)
        { // take things away from box
            dropItemOriginWindow.GetComponent<InventorySlotManager>().InvokeEvent(
                dropItemOriginSlot.transform.GetIndexInParent(), droppedItem, true);
        }
        //Debug.Log("##########Test Variables End##########");
        void exchangeItem()
        {
            if (targetDragItem == null) return;
            targetDragItem.parentAfterDrag = dropDragItem.parentAfterDrag;
            targetDragItem.gameObject.transform.SetParent(dropDragItem.parentAfterDrag);
            dropDragItem.parentAfterDrag = transform;
            PlayerBackpack.SetItemInPlayerBackpack(droppedItem, targetSlotIndex, targetWindowPlaceType);
            PlayerBackpack.SetItemInPlayerBackpack(targetItem, originSlotIndex, originWindowPlaceType);
        }
        void dropItem()
        {
            dropDragItem.parentAfterDrag = transform;
            PlayerBackpack.SetItemInPlayerBackpack(droppedItem, targetSlotIndex, targetWindowPlaceType);
            PlayerBackpack.SetItemInPlayerBackpack(null, originSlotIndex, originWindowPlaceType);
        }
    }

    public void UpdateItemAttribute(ItemData item)
    {
        if (curSlotItem != null) {
            if(item != null)
                Debug.Log($"curSlot: {gameObject.name}, curSlotItem: {curSlotItem.itemName} / reg item: {item.itemName}");
            else 
                Debug.Log($"curSlot: {gameObject.name}, unregister {curSlotItem.itemName}");
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

    private void registerItemAttribute(ItemData item)
    {
        RemoveItemAttribute();
        curSlotItem = item;
        if (item != null)
        {
            //Debug.Log("Register item attribute: " + curSlotItem.name);
            PlayerStats.ChangePlayerStats(item.itemAttributes);
            Player.OnExternalStatChange();
        }
    }

    public void RemoveItemAttribute()
    {
        if (curSlotItem != null)
        {
            //Debug.Log("remove item attribute: " + curSlotItem.name);
            foreach (ItemAttribute itemAttr in curSlotItem.itemAttributes)
            {
                float value = itemAttr.AtrbValue * -1;
                PlayerStats.ChangePlayerStats(new ItemAttribute(itemAttr.AtrbName, value));
            }
            curSlotItem = null;
            Player.OnExternalStatChange();
        }
        else
        {
            //Debug.Log("null item");
        }
    }
}
