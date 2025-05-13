using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    PlayerStateExecutor _executor { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    CombatBuffHandler _buffHandler { get { return PlayerCompManager.TryGetPlayerComp<CombatBuffHandler>(); } }
    ItemData item { get { return itemDetail.item; } }
    ItemDetail itemDetail;
    public void Awake()
    {
        itemDetail = GetComponent<ItemDetail>();
    }
    public void ConsumeItem()
    {
        //Debug.Log($"Consuming item type: {item.consumableItemType}");
        switch (item.consumableItemType)
        {
            case ConsumableItemType.InstantPotion:
                UsePotion(item.itemAttributes);
                break;
            case ConsumableItemType.AntiDote:
                UseAntidoet(item);
                break;
            default:
                UseItemGeneric(item);
                break;
        }
    }

    public void UsePotion(List<ItemAttribute> attributes)
    {
        bool useItem = false;
        if (item.addBuff != null)
        {
            _buffHandler.AddBuff(item.addBuff);
            useItem = true;
        }
        //Debug.Log($"Using item {item.itemName}");
        if (_executor.PlayerCombatStats.CanConsumeItem(attributes))
        {
            _executor.ChangePlayerCombatStat(attributes);
            useItem = true;
        }
        if (useItem)
        {
            _executor.SoundMan.PlaySound("UsePotion");
            DecreaseItemStack();
        }
        else
        {
            Debug.Log("cannot use this item");
        }
    }

    public void UseAntidoet(ItemData item)
    {
        _buffHandler.RemoveAllNerfs();
        _executor.SoundMan.PlaySound("UsePotion");
        DecreaseItemStack();
    }

    public void UseItemGeneric(ItemData item)
    {
        if(item.itemAttributes.Count > 0)
            _executor.ChangePlayerExtraStat(item.itemAttributes);
        DecreaseItemStack();
    }

    void DecreaseItemStack()
    {
        item.curStack--;
        itemDetail.UpdateStack();
        if (item.curStack <= 0)
        {
            Destroy(gameObject);
            PlayerCompManager.TryGetPlayerComp<PlayerBackpack>().RemoveItem(item);
            UI_Controller UIcontroller = UI_Controller.Instance;
            if (UIcontroller.IsUIActive(UI_Window.EquipmentTooltip)){
                UIcontroller.SetUIActive(UI_Window.EquipmentTooltip, false);
            }
        }
    }
}
