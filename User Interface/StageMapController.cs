using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageMapController : MonoBehaviour
{
    public string StartStageID;
    [SerializeField] Transform pivot;
    [SerializeField] float pivotOffset;
    [SerializeField] Button nextLevelButton;

    public StageSettings curStage;
    bool isCurStageInitialized = false;
    PlayerStateExecutor _player { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    StageManager _stageManager { get { return StageManager.Instance; } }
    StageSettings _startStage;

    public StageSettings StartStage 
    { 
        get {
            if (_startStage != null)
            {
                return _startStage;
            }
            _startStage = StageManager.Instance.GetStageByShortID(StartStageID);
            return _startStage;
        } 
    } 

    private void Awake()
    {
        
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        Init();
        Debug.Log("curStage: " + curStage.gameObject.ToString());
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent<StageMapButtons>(out var stageButton))
            {
                stageButton.InitializeButton();
                StageSettings stage = stageButton.GetStage();
                if (stage.IsFinalStage() && nextLevelButton != null)
                {
                    nextLevelButton.gameObject.SetActive(stage.IsStageCleared());
                }
            }
        }
    }
    public void OnExitWindow()
    {
        UI_Controller.OnClosableWindowExit(gameObject);
    }

    public void EnterStage(StageSettings newStage)
    {
        curStage.OnExitStage();
        // if new stage is ahead of stage player is at, move player to new stage's entered point
        // otherwise move to return point(which is the exit point)
        Transform enterPoint;
        if (!newStage.IsStageCleared())
            // force transport to enter point if stage is not clear
            enterPoint = newStage.EnteredPoint;
        else if (curStage.IsParentStageOf(newStage))
            enterPoint = newStage.EnteredPoint;
        else if (newStage.IsStageCleared() && newStage is EventStage)
            enterPoint = newStage.ReturnPoint;
        else
            enterPoint = newStage.ReturnPoint;

        _player.TransportPlayerTo(enterPoint);
        curStage = newStage;
        newStage.OnEnterStage();
        UI_Controller.GetUIScript<InteractObject>().ResetDetector();
        OnExitWindow();
    }

    public void SetPivot(Transform newPosTransform)
    {
        pivot.position = newPosTransform.position;
        Debug.Log($"pivot position set to {newPosTransform.gameObject}");
    }

    public void SetCurStage(StageSettings newStage)
    {
        curStage = newStage;
        isCurStageInitialized = true;
        Debug.Log($"cur stage is {curStage.stageID}, is initialized: {isCurStageInitialized}");
        gameObject.SetActive(true);
        StartCoroutine(ExtendIEnumerator.ActionInNextFrame(() =>
        {
            Debug.Log($"buttons count:{transform.GetChild(0).childCount}");
            StageMapButtons[] buttons = GetComponentsInChildren<StageMapButtons>(true);
            foreach (StageMapButtons button in buttons)
            {
                var stage = button.GetStage();
                Debug.Log($"checking stage: " + stage.stageID);
                if (stage == newStage)
                {
                    SetPivot(button.GetPivotTransform());
                    break;
                }
            }
            gameObject.SetActive(false);
        }));
        newStage.OnEnterStage();
    }

    public void Init()
    {
        if (curStage == null && !isCurStageInitialized)
        {
            curStage = StartStage;
            Debug.Log("Initialize current stage to start stage");
        }
        else
        {
            Debug.Log("Cur stage is already initialized: " + curStage.stageID);
        }
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

    public void GoToPrevLevel()
    {
        int curSceneIndex = LevelManager.CurLevelScene;
        if (curSceneIndex == 0)
        {
            curSceneIndex = gameObject.scene.buildIndex;
        }
        curSceneIndex--;
        LevelManager.LoadLevel(curSceneIndex);
        LevelManager.CurLevelScene = curSceneIndex;
    }
}
