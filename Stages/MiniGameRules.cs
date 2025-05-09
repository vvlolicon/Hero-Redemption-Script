// OpenBook.cs
using UnityEngine;
using System.Collections.Generic;

public class MiniGameRules : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;

    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        UI_Controller.SetUIActive(UI_Window.MiniGameRules, true);
    }
}


/**
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenBook : MonoBehaviour, IInteractableObject
{
    [SerializeField] private string _interactTitle;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    
    [SerializeField] string _title;
    [TextArea, SerializeField] private List<string> _bookContent;

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        Debug.Log("Book Interact");
        UI_Controller.SetUIActive(UI_Window.OpenBook, true);
        BookContentControl control = UI_Controller.GetUIScript<BookContentControl>();
        control.SetBook(_title, _bookContent);
    }
}
**/