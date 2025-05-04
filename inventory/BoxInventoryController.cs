using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoxInventoryController : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] InventorySlotManager manager;
    LootBox curBox;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    private void Start()
    {
        manager.SlotChangeAction += OnItemChange;
    }

    public void OpenBox(LootBox box)
    {
        curBox = box;
        List<ItemData> itemsShow = box.GetItems();
        UI_Controller.OpenInventoryUI(UI_Window.BoxInventoryUI, itemsShow);
    }

    public void OnItemChange(int slotIndex, ItemData item, bool deteleItem)
    {
        curBox.SetItemAtIndex(slotIndex, item, deteleItem);
    }
}
