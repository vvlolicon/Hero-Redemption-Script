using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetail : MonoBehaviour
{
    public Item item;
    private void OnEnable()
    {
        if (item != null)
        {
            transform.GetChild(0).GetComponent<Image>().overrideSprite = item.itemIcon;
        }
    }
}
