using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipWindow : MonoBehaviour
{
    TMP_Text _itemName;
    TMP_Text _itemDesc;
    private void Start()
    {
        _itemName = transform.GetChild(0).GetComponent<TMP_Text>();
        _itemDesc = transform.GetChild(1).GetComponent<TMP_Text>();
        gameObject.SetActive(false);

    }
    private void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            gameObject.transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(Item item)
    {
        gameObject.SetActive(true);

        _itemName.text = item.itemName;
        string itemDesc = item.itemDesc + "\n" + "\n";
        foreach (ItemAttribute attr in item.itemAttributes)
        {
            if (attr.AtrbValue != 0)
            {
                string atrbColor = (attr.AtrbValue > 0) ? "<color=#1A5F16>+" : "<color=red>-";
                itemDesc += attr.AtrbName + ": " + atrbColor + attr.AtrbValue + "</color>" + "\n";
            }
        }
        _itemDesc.text = itemDesc;
    }

    public void DisableTooltip()
    {
        gameObject.SetActive(false);
    }
}
