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
        gameObject.SetActive(true);
        manager.ForceSetSlotNum(itemsShow.Count);
        StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() => {
            for (int i = 0; i < itemsShow.Count; i++)
            {
                manager.SetItemAtSlot(i, itemsShow[i]);
            }
        }));
    }

    public void OnItemChange(int slotIndex, Item item, bool deteleItem)
    {
        curBox.SetItemAtIndex(slotIndex, item, deteleItem);
    }
}
