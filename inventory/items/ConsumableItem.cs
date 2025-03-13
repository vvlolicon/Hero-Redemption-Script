using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ConsumableItem : MonoBehaviour
{
    PlayerStateExecutor _executor;
    Item item;
    ItemDetail itemDetail;
    public void Awake()
    {
        _executor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateExecutor>();
        itemDetail = GetComponent<ItemDetail>();
        item = itemDetail.item;
    }
    public void ConsumeItem()
    {
        switch (item.consumableItemType)
        {
            case ConsumableItemType.InstantPotion:
                UsePotion(item.itemAttributes);
                break;
        }
    }

    public void UsePotion(List<ItemAttribute> attributes)
    {
        if (PlayerBaseMethods.CanConsumeItem(_executor.PlayerStats, attributes))
        {
            PlayerBaseMethods.ChangePlayerStats(_executor.PlayerStats, attributes);
            item.curStack--;
            itemDetail.UpdateStack();
            if (item.curStack == 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //Debug.Log("cannot use this item");
        }
    }
}
