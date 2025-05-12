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
        SceneLoader.Instance.ReturnToMainMenu();
    }

    public void GoToNextLevel()
    {
        int curSceneIndex = LevelManager.CurLevelScene;
        if (curSceneIndex == 0)
        {
            curSceneIndex = gameObject.scene.buildIndex;
        }
        curSceneIndex++;
        LevelManager.LoadLevel(curSceneIndex);
        LevelManager.CurLevelScene = curSceneIndex;
    }
}
