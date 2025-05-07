using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinUIControl : MonoBehaviour
{
    private void OnEnable()
    {
        var animator = GetComponent<Animator>();
        animator.Play("WinUI");
    }

    public void OnClickQuitGame()
    {
        Time.timeScale = 1;
        MainMenuController.Instance.OnReturnMainMenu();
    }
}
