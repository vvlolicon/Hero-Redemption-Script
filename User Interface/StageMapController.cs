using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapController : MonoBehaviour
{
    [SerializeField] StageSettings _startStage;
    [SerializeField] Transform pivot;
    [SerializeField] float pivotOffset;

    [HideInInspector] public StageSettings curStage;
    PlayerStateExecutor _player { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    public StageSettings StartStage => _startStage;

    private void Awake()
    {
        if (curStage == null)
            curStage = _startStage;
    }

    private void OnEnable()
    {
        if (curStage == null)
            curStage = _startStage;
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
        Debug.Log("Initialize current stage to start stage");
        curStage = _startStage;
    }

    
}
