using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        curStage = StartStage;
    }

    private void OnEnable()
    {
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

    public void SetPivot(Transform newPos)
    {
        pivot.localPosition = new Vector3(newPos.localPosition.x, newPos.localPosition.y + pivotOffset, 0);
    }
}
