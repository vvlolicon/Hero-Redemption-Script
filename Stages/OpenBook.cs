using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenBook : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerBackpack PlayerBackpack { get { return GameObjectManager.TryGetPlayerComp<PlayerBackpack>(); } }

    [SerializeField] string _title;
    [TextArea, SerializeField]
    List<string> _bookContent;

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        Debug.Log("Loot Box Interact");
        //UI_Controller.SetUIActive(UI_Window.OpenBook, true);
        BookContentControl control = UI_Controller.GetUIScript<BookContentControl>();
        control.SetBook(_title, _bookContent);
    }

}
