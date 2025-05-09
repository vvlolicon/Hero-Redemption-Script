using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    MainMenuController controller { get { return MainMenuController.Instance; } }
    public void OnClickStart()
    {
        SceneLoader.Instance.StartNewGame();
    }
    public void OnClickExit()
    {
        SceneLoader.Instance.ExitGame();
    }
}
