using Assets.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class MainMenuController : SingletonDDOL<MainMenuController>
{
    private void Start()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(gameObject, thisScene);
        StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
        {
            VolumeMaster volumeMaster = FindObjectOfType<VolumeMaster>();
            volumeMaster.InitializeSliders();
        }));
    }
}
