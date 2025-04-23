using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class InventorySlotManager : MonoBehaviour
{
    public int slot_num = 0;
    [SerializeField] GameObject slotObject;
    [SerializeField] GameObject itemObject;

    public event Action<int, Item> SlotChangeAction;

    private void Awake()
    {
    }
    private void OnEnable()
    {
        int curSlotNum = transform.childCount;
        // check number of slots
        if (slot_num != curSlotNum)
        {
            SetSlotNum(slot_num);
        }
    }

    public void InvokeEvent(int slotIndex, Item item)
    {
        if(gameObject.CompareTag("Box_Inventory"))
            SlotChangeAction?.Invoke(slotIndex, item);
    }

    public void SetSlotNum(int value)
    {
        ChangeSlotNum(value - slot_num);
    }

    public void ForceSetSlotNum(int value)
    {
        ChangeSlotNum(value - slot_num, true);
    }

    public void ChangeSlotNum(int value, bool forceDeleteItem = false)
    {
        Debug.Log($"Change slot num to {value}");
        if (value == 0) return;
        slot_num += value;
        float slot_row = Mathf.Floor(slot_num / 5);
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, slot_row * 55 + 15);
        if (value > 0)
        {
            // spawn extra slot for the inventory
            for (int i = 0; i < value; i++)
            {
                GameObject newSlot = Instantiate(slotObject, transform);
                GameObject newItem = Instantiate(itemObject, newSlot.transform);
            }
        }
        else
        {
            int decrease_value = value * -1;
            int deleteSlotAt = transform.childCount-1;
            int destroySlots = 0;
            bool isDestroySlot;
            for (int i = 0 ; i < decrease_value; i++)
            {
                isDestroySlot = false;
                while (!isDestroySlot && deleteSlotAt > 0)
                {
                    // if the slot does not have item inside, destroy it, otherwise find next one until a slot dont have item.
                    var slot = transform.GetChild(deleteSlotAt);
                    var itemScript = slot.GetComponentInChildren<ItemDetail>();
                    if (slot.childCount == 0 || itemScript.item == null)
                    {
                        Destroy(slot.gameObject);
                        destroySlots++;
                        isDestroySlot = true;
                    }
                    deleteSlotAt--;
                }
            }
            if(destroySlots < decrease_value)
            {
                Debug.Log("slots left because item occupied");
                if (forceDeleteItem)
                {
                    deleteSlotAt = transform.childCount - 1;
                    for (int i = 0; i < decrease_value - destroySlots; i++)
                    {
                        Debug.Log($"Deleteing slot At {deleteSlotAt}");
                        var slot = transform.GetChild(deleteSlotAt);
                        var itemScript = slot.GetComponentInChildren<ItemDetail>();
                        Destroy(slot.gameObject);
                        deleteSlotAt--;
                    }
                }
            }
        }
    }

    public void SetItemAtSlot(int slotIndex, Item item)
    {
        int curSlotNum = transform.childCount;
        if (slotIndex > curSlotNum - 1)
        {
            int diffSlotNum = slotIndex - (curSlotNum - 1);
            ChangeSlotNum(diffSlotNum);
        }
        SetSlotItem(slotIndex, item);
    }

    void SetSlotItem(int slotIndex, Item item)
    {
        Transform slot = transform.GetChild(slotIndex);
        if (slot.childCount > 0)
        {
            slot.GetComponentInChildren<ItemDetail>().SetItem(item);
        }
        else
        { // create item if there is no item in the slot
            GameObject newItem = Instantiate(itemObject, slot.transform);
            newItem.GetComponent<ItemDetail>().SetItem(item);
        }
        //
    }

}
