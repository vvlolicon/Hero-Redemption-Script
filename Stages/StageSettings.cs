using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StageSettings : MonoBehaviour
{
    public List<StageSettings> _childStages = new List<StageSettings>();
    public List<PickupSpawner> _pickups = new List<PickupSpawner>();
    protected bool _isLocked = true;
    protected bool _isRootStage = false;
    protected bool _isFinalStage = false;
    protected bool _stageCleared = false;
    protected bool _isDiscovered = false;

    public Transform EnteredPoint;
    public Transform ReturnPoint;
    public Collider ExitPointCollider;
    [Header("Stage Identity")]
    public string stageID;
    PlayerStateExecutor _player { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }

    protected virtual void SpawnPickups()
    {
        if (_pickups.Count == 0) return;
        for (int i = 0; i < _pickups.Count; i++)
        {
            _pickups[i].SpawnPickup();
        }
    }

    protected void ClearStage() 
    {
        _stageCleared = true;
        UnlockChildStages();
        OnStageClear();
    }

    public virtual void InitStage()
    {
        SpawnPickups();
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
    public bool IsStageDiscovered() => _isDiscovered;
    public bool IsStageCleared() => _stageCleared;
    public bool IsFinalStage() => _isFinalStage;
    public bool IsParentStageOf(StageSettings stage)
    {
        foreach(StageSettings childStage in _childStages)
        {
            if (stage == childStage) return true;
            else IsParentStageOf(childStage);
        }
        return false;
    }

    public virtual void SetStage(bool isLocked, bool isDiscovered, bool isCleared, List<bool> isPickupsPicked)
    {
         _isLocked = isLocked;
         _isDiscovered = isDiscovered;
        for (int i = 0; i < _pickups.Count; i++)
        {
            _pickups[i].PickedItem = isPickupsPicked[i];
            if (!isPickupsPicked[i])
            {
                _pickups[i].SpawnPickup();
            }
        }
        if (isCleared)
        {
            ClearStage();
        }
    }
    public List<string> GetChildStageIDs()
    {
        return _childStages.ConvertAll(s => s.stageID);
    }
    public List<bool> GetIsPickupsPicked() => _pickups.ConvertAll(p => p.PickedItem);
}

[System.Serializable]
public class GeneralStageData
{
    public string stageID;
    public List<bool> pickedPickups;

    public bool isLocked;
    public bool isDiscovered;
    public bool isCleared;
}
