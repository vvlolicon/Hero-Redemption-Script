using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour, IInteractableObject
{
    [SerializeField] string _interactTitle;
    public delegate void InteractEvent();
    public event InteractEvent OnDoorInteract;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    public string GetInterableTitle()
    {
        return _interactTitle;
    }

    public void Interact()
    {
        Debug.Log("Door Interact");
        UI_Controller.SetUIActive(UI_Window.StageMap, true);
    }
}
