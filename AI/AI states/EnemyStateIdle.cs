using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateIdle : EnemyBaseStates
{

    public EnemyStateIdle(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor){}

    public override AIStates CurStateType() { return AIStates.IDLE; }

    public override void EnterState()
    {
        Executor.Animator.SetTrigger("isIdle");
        Executor.WaitTimer = 0;
    }

    protected override void CheckSwitchState()
    {
        if (Methods.CanSeePlayer())
        {
            SwitchState(StateMan.Chase());
        }
        else if (Executor.PatrolPoints.Count > 0)
        {
            SwitchState(StateMan.Patrol());
        }
    }

    protected override void UpdateState()
    {
        
    }

    protected override void ExitState()
    {
        Executor.Animator.ResetTrigger("isIdle");
    }

    protected override void InitializeSubStates()
    {
        
    }

}
