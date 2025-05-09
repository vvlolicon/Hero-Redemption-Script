using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseUIControl : MonoBehaviour
{
    private void OnEnable()
    {
        var animator = GetComponent<Animator>();
        animator.Play("LoseUI");
    }

    public void OnClickQuitGame()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.ReturnToMainMenu();
    }

}
