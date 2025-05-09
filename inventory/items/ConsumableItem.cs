using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    PlayerStateExecutor _executor;
    ItemData item { get { return itemDetail.item; } }
    ItemDetail itemDetail;
    public void Awake()
    {
        _executor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateExecutor>();
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
            default:
                UseItemGeneric(item.itemAttributes);
                break;
        }
    }

    public void UsePotion(List<ItemAttribute> attributes)
    {
        //Debug.Log($"Using item {item.itemName}");
        if (_executor.PlayerCombatStats.CanConsumeItem(attributes))
        {
            _executor.SoundMan.PlaySound("UsePotion");
            _executor.ChangePlayerCombatStat(attributes);
            DecreaseItemStack();
        }
        else
        {
            Debug.Log("cannot use this item");
        }
    }

    public void UseItemGeneric(List<ItemAttribute> attributes)
    {
        _executor.ChangePlayerExtraStat(attributes);
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
