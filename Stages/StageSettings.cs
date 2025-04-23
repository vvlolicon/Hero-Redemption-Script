using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StageSettings : MonoBehaviour
{
    public List<StageSettings> _childStages = new List<StageSettings>();
    protected bool _isLocked = true;
    protected bool _isRootStage;
    protected bool _isFinalStage;
    protected bool _stageCleared = false;
    protected bool _isDiscovered;

    public Transform EnteredPoint;
    public Transform ReturnPoint;
    public Collider ExitPointCollider;
    PlayerStateExecutor _player { get { return GameObjectManager.TryGetPlayerComp<PlayerStateExecutor>(); } }

    protected void ClearStage() 
    {
        _stageCleared = true;
        UnlockChildStages();
        OnStageClear();
    }

    public virtual void OnStageClear() {
        if (ExitPointCollider != null)
            ExitPointCollider.enabled = true;
    }
    public virtual void OnEnterStage() {
        if (ExitPointCollider != null)
            ExitPointCollider.enabled = _stageCleared;
    }
    public virtual void OnExitStage() {
        if (!_stageCleared)
        {
            ResetStage();
        }
    }

    protected void UnlockChildStages()
    {
        foreach(StageSettings stage in _childStages)
        {
            stage._isLocked = false;
        }
    }

    protected virtual void ResetStage()
    {
        _stageCleared = false;
        foreach (StageSettings stage in _childStages)
        {
            stage._isLocked = true;
        }
    }

    public bool IsStageLocked() => _isLocked;
    public bool IsParentStageOf(StageSettings stage)
    {
        foreach(StageSettings childStage in _childStages)
        {
            if (stage == childStage) return true;
            else IsParentStageOf(childStage);
        }
        return false;
    }
}


