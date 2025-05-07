using System.Collections.Generic;
using UnityEngine;

public class NPC_SellItem : NPC_Base, IInteractableObject
{
    [SerializeField] List<Item> ItemsSold;

    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerBackpack PlayerBackpack { get { return PlayerCompManager.TryGetPlayerComp<PlayerBackpack>(); } }

    public new void Interact()
    {
        UI_Controller.SetUIActive(UI_Window.BuyItemUI, true);
        UI_Controller.GetUIScript<BuyItemUIController>().SetUI(ItemsSold, NPC_Name);
        UI_Controller.OpenInventoryUI(UI_Window.InventoryUI, PlayerBackpack.GetPlayerBackpackItems());
    }
}