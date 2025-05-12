using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;
    [SerializeField] List<Item> _containItems = new List<Item>();
    [SerializeField] int _money;
    bool opened = false;
    List<ItemData> _containItemDatas = new();
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerBackpack PlayerBackpack { get { return PlayerCompManager.TryGetPlayerComp<PlayerBackpack>(); } }

    private void Start()
    {
        foreach (Item item in _containItems)
        {
            _containItemDatas.Add(item.GetItemDataClone());
        }
    }

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        if (!opened)
        {
            PlayerBackpack.PlayerOwnedMoney += _money;
            UI_Controller.PopMessage($"You gain {_money}$");
            opened = true;
        }
        UI_Controller.GetUIScript<BoxInventoryController>().OpenBox(this);
        UI_Controller.OpenInventoryUI(UI_Window.InventoryUI, PlayerBackpack.GetPlayerBackpackItems());
    }

    public List<ItemData> GetItems() => _containItemDatas;
    public bool IsBoxOpened() => opened;

    public void SetItemAtIndex(int index, ItemData item, bool deleteItem = false)
    {
        if (deleteItem)
        {
            _containItemDatas.Remove(item);
            return;
        }
        _containItemDatas[index] = item;
    }

    public void RemoveItemAtIndex(int index)
    {
        _containItemDatas.RemoveAt(index);
    }

    public void SetBox(List<ItemData> items, bool opened)
    {
        _containItemDatas = items;
        this.opened = opened;
    }
}
