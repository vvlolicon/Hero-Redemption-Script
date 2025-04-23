using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxInventoryController : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] InventorySlotManager manager;
    LootBox curBox;

    private void Start()
    {
        manager.SlotChangeAction += OnItemChange;
    }

    public void OpenBox(LootBox box)
    {
        curBox = box;
        List<Item> itemsShow = box.GetItems();
        manager.ForceSetSlotNum(itemsShow.Count);
        for(int i = 0; i< itemsShow.Count; i++)
        {
            manager.SetItemAtSlot(i, itemsShow[i]);
        }
        gameObject.SetActive(true);
    }

    public void OnItemChange(int slotIndex, Item item)
    {
        curBox.SetItemAtIndex(slotIndex, item);
    }
}
