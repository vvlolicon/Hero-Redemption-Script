using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMapButtons : MonoBehaviour
{
    [SerializeField] string _stageID;
    [SerializeField] Transform _pivotPos;

    StageSettings _stage;
    Button _stageButton;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    StageMapController _mapController { get{ return UI_Controller.GetUIScript<StageMapController>(); } }
    StageManager _stageManager { get { return StageManager.Instance; } }

    private void Awake()
    {
        _stageButton = GetComponentInChildren<Button>();
        _stageButton.onClick.AddListener(OnEnterStage);
    }

    public void InitializeButton()
    {
        _stage = _stageManager.GetStageByShortID(_stageID);
        if (_stageButton == null)
        {
            gameObject.SetActive(true);
            _stageButton = GetComponentInChildren<Button>();
        }
        gameObject.SetActive(!_stage.IsStageLocked());
        if (_mapController.curStage!= null)
            _stageButton.interactable = !(_mapController.curStage == _stage);
    }

    public void OnEnterStage()
    {
        _mapController.EnterStage(_stage);
        if(_pivotPos!= null)
            _mapController.SetPivot(_pivotPos);
        else
            _mapController.SetPivot(transform);
    }

    public Transform GetPivotTransform() => (_pivotPos == null)? transform : _pivotPos;
    public StageSettings GetStage() => _stage;
}
