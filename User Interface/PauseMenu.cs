using Assets.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    
    public void OnExitWindow()
    {
        Time.timeScale = 1;
        UI_Controller.OnClosableWindowExit(gameObject);
    }

    public void OnExitGame()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.ReturnToMainMenu();
    }

    public void OnSaveGame()
    {
        string path = SaveSystem.SaveGame("New Save");
        UI_Controller.PopMessage($"Game successfully saved to {path}");
        OnExitWindow();
    }
}
