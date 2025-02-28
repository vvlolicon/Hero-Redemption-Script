using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateGrounded : EnemyBaseStates
{
    public EnemyStateGrounded(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor)
    {
        IsRootState = true; 
    }

    public override AIStates CurStateType() { return AIStates.GROUND; }
    public override void EnterState()
    {
        InitializeSubStates();
        SubState.EnterState();
        Executor.Agent.isStopped = false;
    }

    protected override void CheckSwitchState()
    {
    }

    protected override void UpdateState()
    {
    }

    protected override void InitializeSubStates()
    {
        if (Methods.CanSeePlayer())
        {
            SetSubState(StateMan.Chase());
        }
        else if (Executor.PatrolPoints.Count > 0)
        {
            SetSubState(StateMan.Patrol());
        }
        else
        {
            SetSubState(StateMan.Idle());
        }
    }
}
