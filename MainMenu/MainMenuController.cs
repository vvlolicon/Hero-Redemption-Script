using System.Collections;
using System.Collections.Generic;
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
            DontDestroyOnLoad(gameObject);
        }));
    }
    public void OnClickPlaygame()
    {
        StartCoroutine(LoadGame());
    }

    public void OnReturnMainMenu()
    {
        StartCoroutine(ReturnToMainMenu());
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    IEnumerator LoadGame()
    {
        FindObjectOfType<VolumeMaster>().RemoveLiseners();
        DontDestroyOnLoad(gameObject);
        // load new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        Destroy(FindAnyObjectByType<AudioListener>().gameObject);
        Destroy(FindAnyObjectByType<EventSystem>().gameObject);
        asyncLoad.allowSceneActivation = true;
        // wait for scene to load
        while (!asyncLoad.isDone)
        {
            yield return new WaitForFixedUpdate(); //delay for a fixed update frame;
        }
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(0)); // unload old scene
        while (!asyncUnload.isDone)
        {
            yield return new WaitForFixedUpdate(); //delay for a fixed update frame;
        }
        PlayerStateExecutor executor = GameObjectManager.TryGetPlayerComp<PlayerStateExecutor>();
        executor.InitializePlayer();
        UI_Controller.Instance.Initialize();
        VolumeMaster volumeMaster = FindObjectOfType<VolumeMaster>();
        volumeMaster.ResetSliders();
    }
    IEnumerator ReturnToMainMenu()
    {
        FindObjectOfType<VolumeMaster>().RemoveLiseners();
        DontDestroyOnLoad(gameObject);
        // load new scene
        var asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForFixedUpdate(); //delay for a fixed update frame;
        }
        yield return new WaitForSeconds(0.2f);
        VolumeMaster volumeMaster = FindObjectOfType<VolumeMaster>();
        volumeMaster.ResetSliders();
    }
}
