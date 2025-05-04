using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    TooltipWindow tooltipWindow { get { return UI_Controller.GetUIScript<TooltipWindow>(); } }

    private void Start()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemData curPointItem = GetComponent<ItemDetail>().item;
        tooltipWindow?.ShowTooltip(curPointItem);
        if (transform.parent.parent != null && transform.parent.parent.CompareTag("Player_HotbarItem"))
        {
            tooltipWindow.GetComponent<RectTransform>().SetPivot(new Vector2(0.5f, 0));
        } 
        else
        {
            tooltipWindow.GetComponent<RectTransform>().SetPivot(new Vector2(0, 1));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipWindow?.DisableTooltip();
    }
}
