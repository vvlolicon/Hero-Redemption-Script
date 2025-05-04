using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapController : MonoBehaviour
{
    [SerializeField] StageSettings StartStage;
    [SerializeField] Transform pivot;
    [SerializeField] float pivotOffset;

    public StageSettings curStage {get; private set;}
    PlayerStateExecutor _player { get { return GameObjectManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    private void Awake()
    {
        //if(SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    Init();
        //}
    }

    private void OnEnable()
    {
        if (curStage == null)
            curStage = StartStage;
        Debug.Log("curStage: " + curStage.gameObject.ToString());
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent<StageMapButtons>(out var stageButton))
                stageButton.InitializeButton();
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
        if (curStage.IsParentStageOf(newStage))
        {
            _player.TransportPlayerTo(newStage.EnteredPoint.position);
        }
        else
        {
            _player.TransportPlayerTo(newStage.ReturnPoint.position);
        }
        curStage = newStage;
        newStage.OnEnterStage();
        UI_Controller.GetUIScript<InteractObject>().ResetDetector();
        OnExitWindow();
    }

    public void SetPivot(Transform newPosTransform)
    {
        pivot.position = newPosTransform.position;
    }

    public void SetCurStage(StageSettings newStage)
    {
        curStage = newStage;
        StageMapButtons[] buttons = GetComponentsInChildren<StageMapButtons>();
        foreach (StageMapButtons button in buttons)
        {
            if(button._stage == newStage)
            {
                pivot.position = button.GetPivotTransform().position;
                break;
            }
        }
        newStage.OnEnterStage();
    }

    public void Init()
    {
        curStage = StartStage;
    }

    
}
