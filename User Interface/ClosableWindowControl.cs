using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosableWindowControl : MonoBehaviour
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    PlayerInputData Inputdata { get { return PlayerInputData.Instance; } }

    public void OnExitWindow()
    {
        UI_Controller.OnClosableWindowExit(gameObject);
    }
}
