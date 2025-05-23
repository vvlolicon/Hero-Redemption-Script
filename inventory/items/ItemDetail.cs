using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetail : MonoBehaviour
{
    public ItemData item;

    ConsumableItem _consumeScript;
    private void OnEnable()
    {
        UpdateItem();
    }

    public void SetItem(ItemData item)
    {
        this.item = item;
        UpdateItem();
    }

    public void UpdateItem()
    {
        if (item != null)
        {
            transform.GetChild(0).GetComponent<Image>().overrideSprite = item.itemIcon;
            if (item.maxStack > 1)
            {
                setStackText();
            }
            else
            {
                transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
            if (item.curStack > item.maxStack)
            {
                item.curStack = item.maxStack;
            }
        }
    }

    public void UpdateStack()
    {
        setStackText();
    }

    void setStackText()
    {
        TMP_Text stackText = transform.GetChild(1).GetComponent<TMP_Text>();
        if (item.curStack > 99)
        {
            stackText.text = "99+";
        }
        else
        {
            stackText.text = "" + item.curStack;
        }
    }
}
