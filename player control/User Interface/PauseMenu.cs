using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    public void OnExitWindow()
    {
        Time.timeScale = 1;
        UI_Controller.OnClosableWindowExit(gameObject);
    }
}
