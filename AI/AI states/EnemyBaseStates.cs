using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseStates : PlayerBaseState
{
    EnemyStateManager _manager;
    EnemyStateExecutor _executor;
    EnemyBaseStates _curSubState;
    AIMethods _AIMethods;
    protected new EnemyStateManager StateMan { get { return _manager; } }
    protected new EnemyStateExecutor Executor { get { return _executor; } }
    protected new EnemyBaseStates SuperState { get; set; }
    public new EnemyBaseStates SubState { get { return _curSubState; } }
    protected AIMethods Methods { get { return _AIMethods; } }

    public EnemyBaseStates(EnemyStateManager manager, EnemyStateExecutor executor)
    {
        _manager = manager;
        _executor = executor;
        _AIMethods = new AIMethods(executor);
    }

    public new virtual AIStates CurStateType() { return AIStates.IDLE; }

    public new void UpdateStates()
    {
        PreUpdateState();
        _curSubState?.PreUpdateState();

        ExecuteUpdateMethods();
        _curSubState?.ExecuteUpdateMethods();
    }

    public void SwitchState(EnemyBaseStates newState)
    {

        ExitState();
        newState.InitializeSubStates();
        newState.EnterState();

        if (IsRootState)
        {
            Executor.CurState = newState;
        }
        else if (SuperState != null)
        {
            SuperState.SetSubState(newState);
        }
    }
    protected void SetSubState(EnemyBaseStates newSubState)
    {
        _curSubState = newSubState;
        newSubState.SuperState = this;
    }

    protected new void ExecuteUpdateMethods()
    {
        UpdateState();
        CheckSwitchState();
    }

}
