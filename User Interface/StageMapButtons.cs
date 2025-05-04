using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMapButtons : MonoBehaviour
{
    public StageSettings _stage;    
    [SerializeField] Transform _pivotPos;
    Button _stageButton;
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }
    StageMapController _mapController { get{ return UI_Controller.GetUIScript<StageMapController>(); } }

    private void Awake()
    {
        _stageButton = GetComponentInChildren<Button>();
        _stageButton.onClick.AddListener(OnEnterStage);
    }

    public void InitializeButton()
    {
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
}
