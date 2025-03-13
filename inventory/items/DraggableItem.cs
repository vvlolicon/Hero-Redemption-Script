using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Image image;

    void OnEnable()
    {
        parentAfterDrag = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image = GetComponentInChildren<Image>();
        //Debug.Log("start drag" + eventData.pointerDrag.name);
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("draging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("end drag" + eventData.pointerDrag.name);
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        SetItemShowScale(gameObject);
    }

    public static void SetItemShowScale(GameObject item)
    {
        GameObject parentWindow = item.transform.parent.parent.gameObject;
        //Debug.Log("parentWindow: " + parentWindow.name);
        if (parentWindow.CompareTag("Player_HotbarItem"))
        {
            item.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            item.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
}
