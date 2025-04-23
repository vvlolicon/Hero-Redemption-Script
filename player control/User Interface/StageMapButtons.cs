using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMapButtons : MonoBehaviour
{
    public StageSettings _stage;
    Button _stageButton;
    [SerializeField] StageMapController _mapController;

    private void Awake()
    {
        _stageButton = GetComponent<Button>();
        _stageButton.onClick.AddListener(OnEnterStage);
    }
    private void OnEnable()
    {
        _stageButton.gameObject.SetActive(!_stage.IsStageLocked());
        _stageButton.interactable = !(_mapController.curStage == _stage);
    }

    public void OnEnterStage()
    {
        _mapController.EnterStage(_stage);
        _mapController.SetPivot(transform);
    }
}
