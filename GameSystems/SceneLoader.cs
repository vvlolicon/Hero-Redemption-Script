using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonDDOL<SceneLoader>
{
    public void LoadScene(int loadedSceneIndex, System.Action actionAfterLoad)
    {
        Debug.LogWarning("Start loading scene");
        StartCoroutine(LoadGameScene(loadedSceneIndex, actionAfterLoad));
    }
    public void StartNewGame()
    {
        StartGamePreActions();
        StartCoroutine(LoadGameScene(1, () => { LevelManager.StartNewGame(); }));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public IEnumerator LoadGameScene(int loadedSceneIndex, System.Action actionAfterLoad)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadedSceneIndex, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.1f);
        actionAfterLoad();
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
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

    void StartGamePreActions()
    {
        FindObjectOfType<VolumeMaster>().RemoveLisenersForSliders();
        Destroy(FindAnyObjectByType<AudioListener>().gameObject);
        Destroy(FindAnyObjectByType<EventSystem>().gameObject);
    }
}