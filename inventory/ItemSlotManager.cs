using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemSlotManager : MonoBehaviour, IDisplayItem
{
    [SerializeField] GameObject itemObject;
    public void SetItemAtSlot(ItemData item, int slotIndex)
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
