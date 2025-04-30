using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    MainMenuController controller { get { return MainMenuController.Instance; } }
    public void OnClickStart()
    {
        controller.OnClickPlaygame();
    }
    public void OnClickExit()
    {
        controller.OnClickExit();
    }
}
