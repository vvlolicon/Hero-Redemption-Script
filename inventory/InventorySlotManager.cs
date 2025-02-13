using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class InventorySlotManager : MonoBehaviour
{
    public int slot_num;
    public GameObject slotObject;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        int curSlotNum = transform.childCount;
        // check number of slots
        if (slot_num != curSlotNum)
        {
            int diffSlotNum = slot_num - curSlotNum;
            slot_num -= diffSlotNum;
            changeSlotNum(diffSlotNum);
        }
        
    }

    public void changeSlotNum(int value)
    {
        slot_num += value;
        float slot_row = Mathf.Floor(slot_num / 5);
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, slot_row * 55 + 15);
        if (value > 0)
        {
            // spawn extra slot for the inventory
            for (int i = 0; i < value; i++)
            {
                GameObject newSlot = Instantiate(slotObject);
                newSlot.transform.SetParent(this.transform);
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
                    if (transform.GetChild(deleteSlotAt).childCount == 0)
                    {
                        Destroy(transform.GetChild(deleteSlotAt).gameObject);
                        destroySlots++;
                        isDestroySlot = true;
                    }
                    deleteSlotAt--;
                }
            }
            if(destroySlots != decrease_value)
            {
                Debug.Log("cannot delete slots because item occupied");
            }
        }
    }
}
