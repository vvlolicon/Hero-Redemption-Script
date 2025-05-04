// OpenBook.cs
using UnityEngine;
using System.Collections.Generic;

public class OpenBook : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;
    [SerializeField] List<Sprite> _bookImage;
    [TextArea, SerializeField] List<string> _title;
    [TextArea, SerializeField] List<string> _bookContent;

    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        UI_Controller.SetUIActive(UI_Window.OpenBook, true);
        BookContentControl control = UI_Controller.GetUIScript<BookContentControl>();
        control.SetBook(_title, _bookContent, _bookImage);
    }
}