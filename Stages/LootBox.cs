using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;
    [SerializeField] List<Item> _containItems = new List<Item>();
    [SerializeField] int _money;
    bool opened = false;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerBackpack PlayerBackpack { get { return GameObjectManager.TryGetPlayerComp<PlayerBackpack>(); } }

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        Debug.Log("Loot Box Interact");
        if (!opened)
        {
            PlayerBackpack.PlayerOwnedMoney += _money;
            UI_Controller.GetUIScript<BoxInventoryController>().OpenBox(this);
            UI_Controller.SetUIActive(UI_Window.InventoryUI, true);
        }
    }

    public List<Item> GetItems() => _containItems;
    public void SetItemAtIndex(int index, Item item)
    {
        if (item == null)
        {
            _containItems.RemoveAt(index);
            return;
        }
        _containItems[index] = item;
    }
}
