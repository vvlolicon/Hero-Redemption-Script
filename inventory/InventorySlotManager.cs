using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public interface IDisplayItem
{
    public void SetItemAtSlot(ItemData item, int slotIndex);
    public void UpdateItemAtSlot(int slotIndex);
}

public class InventorySlotManager : MonoBehaviour, IDisplayItem
{
    public int slot_num = 0;
    [SerializeField] GameObject slotObject;
    [SerializeField] GameObject itemObject;
    public event Action<int, ItemData, bool> SlotChangeAction;

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

    public void InvokeEvent(int slotIndex, ItemData item, bool deleteItem = false)
    {
        if(gameObject.CompareTag("Box_Inventory"))
            SlotChangeAction?.Invoke(slotIndex, item, deleteItem);
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
            if (forceDeleteItem)
            {
                Debug.Log($"transform.childCount: {transform.childCount}, Destroy slot number : " + deleteSlotAt);
                for (int i = 0; i < decrease_value; i++)
                {
                    Debug.Log($"Deleteing slot At {deleteSlotAt}");
                    DestroySlot(deleteSlotAt, true);
                    deleteSlotAt--;
                }
                return;
            }
            for (int i = 0 ; i < decrease_value; i++)
            {
                while (deleteSlotAt > 0)
                {
                    // if the slot does not have item inside, destroy it, otherwise find next one until a slot dont have item.
                    var slot = transform.GetChild(deleteSlotAt);
                    if (DestroySlot(deleteSlotAt))
                    {
                        destroySlots++;
                        break;
                    }
                    deleteSlotAt--;
                }
            }
            if(destroySlots < decrease_value)
            {
                Debug.Log("slots left because item occupied");
            }
        }
    }

    bool DestroySlot(int slotIndex, bool forceDelete = false)
    {
        var slot = transform.GetChild(slotIndex);
        if (forceDelete)
        {
            Debug.Log($"Deleteing slot At {slotIndex}");
            Destroy(slot.gameObject);
            return true;
        }
        var itemScript = slot.GetComponentInChildren<ItemDetail>();
        if (slot.childCount == 0 || itemScript.item == null)
        {
            Debug.Log($"Deleteing slot At {slotIndex}");
            StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() => {
                slot.gameObject.SetActive(false);
                Destroy(slot.gameObject);
            }));
            return true;
        }
        return false;
    }

    public void SetItemAtSlot(ItemData item, int slotIndex)
    {
        int curSlotNum = transform.childCount;
        if (slotIndex > curSlotNum - 1)
        {
            int diffSlotNum = slotIndex - (curSlotNum - 1);
            ChangeSlotNum(diffSlotNum);
        }
        SetSlotItem(item, slotIndex);
    }
    public void UpdateItemAtSlot(int slotIndex)
    {
        var slot = transform.GetChild(slotIndex);
        if (slot.transform.childCount == 0) return;
        slot.GetComponentInChildren<ItemDetail>().UpdateItem();
    }

    void SetSlotItem(ItemData item, int slotIndex)
    {
        Transform slot = transform.GetChild(slotIndex);
        if (slot.childCount > 0)
        {
            if (item == null)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
            else
            {
                slot.GetComponentInChildren<ItemDetail>().SetItem(item);
            }
        }
        else if (item != null)
        { // create item if there is no item in the slot
            GameObject newItem = Instantiate(itemObject, slot.transform);
            newItem.GetComponent<ItemDetail>().SetItem(item);
        }
    }
}
