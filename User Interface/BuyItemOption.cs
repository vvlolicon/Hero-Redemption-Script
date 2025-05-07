using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemOption : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemPrice;
    public Button BuyItemButton;
    Item item;

    public void SetOption(Item item)
    {
        this.item = item;
        itemImage.sprite = item.itemIcon;
        itemName.text = item.itemName;
        itemPrice.text = item.itemValue + "$";
    }

    public ItemData GetBuyItemData() => item.GetItemDataClone();

    public bool CanAffordItem(int deposit) => deposit >= item.itemValue;
}
