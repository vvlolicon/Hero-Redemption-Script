using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    bool _isRootState = false;
    PlayerStatsManager _stateMan;
    PlayerStateExecutor _executor;
    PlayerBaseState _curSubState;
    PlayerBaseMethods _methods;

    // getter / setter
    protected bool IsRootState { get { return _isRootState; } set { _isRootState = value; } }
    protected PlayerBaseState SuperState { get; set; }
    public PlayerBaseState SubState { get { return _curSubState; }}
    protected PlayerStateExecutor Executor { get { return _executor; }}
    protected PlayerStatsManager StateMan { get { return _stateMan; }}
    protected PlayerBaseMethods PlayerMethods { get { return _methods; }}

    public PlayerBaseState(PlayerStatsManager manager, PlayerStateExecutor executor)
    {
        _stateMan = manager;
        _executor = executor;
        _methods = new PlayerBaseMethods(executor);
    }
    // use for enemy AI
    public PlayerBaseState() { }
    protected abstract void CheckSwitchState();
    protected abstract void UpdateState();
    public virtual PlayerStates CurStateType() { return PlayerStates.GROUNDED;  }
    public abstract void EnterState();
    protected virtual void ExitState() {}
    protected virtual void InitializeSubStates() { }
    protected virtual void PreUpdateState() { }

    public void UpdateStates() {
        PreUpdateState();
        _curSubState?.PreUpdateState();

        ExecuteUpdateMethods();
        _curSubState?.ExecuteUpdateMethods();
    }

    public void SwitchState(PlayerBaseState newState) 
    {
        //Vector3 modelPos = Executor.transform.position;
        //modelPos.y -= 0.2f;
        //Executor.ChildPlayer.transform.position = modelPos;
        ExitState();
        //Executor.Animator.SetTrigger("Exit");
        newState.InitializeSubStates();
        newState.EnterState();


        if (IsRootState) { 
            Executor.CurState = newState;
        }
        else if(SuperState != null)
        {
            SuperState.SetSubState(newState);
        }
    }

    protected void SetSubState(PlayerBaseState newSubState) 
    {
        _curSubState = newSubState;
        newSubState.SuperState = this;
    }

    protected void ExecuteUpdateMethods()
    {
        UpdateState();
        CheckSwitchState();
    }

    
}
