using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        //Debug.Log("Collide with object " + other.gameObject.name);
        if (other.TryGetComponent<IInteractableObject>(out _))
        {
            UI_Controller.GetUIScript<InteractObject>().AddInteractObject(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        //Debug.Log("Exit Collide with object " + other.gameObject.name);
        if (other.TryGetComponent<IInteractableObject>(out _))
        {
            UI_Controller.GetUIScript<InteractObject>().RemoveInteractObject(other.gameObject);
        }
    }
    
}
