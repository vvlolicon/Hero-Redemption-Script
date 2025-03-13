using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TooltipWindow tooltipWindow { get; private set; }

    private void OnEnable()
    {
        // assign if null
        tooltipWindow ??= GameObject.FindGameObjectWithTag("Tooltip_Window").transform.GetChild(0).GetComponent<TooltipWindow>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Item curPointItem = GetComponent<ItemDetail>().item;
        tooltipWindow?.ShowTooltip(curPointItem);
        if (transform.parent.parent != null && transform.parent.parent.CompareTag("Player_HotbarItem"))
        {
            PlayerBaseMethods.SetPivot(tooltipWindow.GetComponent<RectTransform>(), new Vector2(0.5f, 0));
        } else
        {
            PlayerBaseMethods.SetPivot(tooltipWindow.GetComponent<RectTransform>(), new Vector2(0, 1));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipWindow?.DisableTooltip();
    }
}
