using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMapButtons : MonoBehaviour
{
    public StageSettings _stage;
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
        gameObject.SetActive(!_stage.IsStageLocked());
        _stageButton.interactable = !(_mapController.curStage == _stage);
    }

    public void OnEnterStage()
    {
        _mapController.EnterStage(_stage);
        _mapController.SetPivot(transform);
    }
}
