using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyStateIdle : EnemyBaseStates
{
    public EnemyStateIdle(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor){}

    public override AIStates CurStateType() { return AIStates.IDLE; }

    public override void EnterState()
    {
        Executor.Animator.SetTrigger("IsIdle");
        Executor.WaitTimer = 0;
        Vector3 distance = Executor.gameObject.transform.position - Executor.PatrolPoints[0].position;
        if (distance.magnitude > 1)
        {
            Executor.Agent.isStopped = false;
            Executor.Agent.SetDestination(Executor.PatrolPoints[0].position);
        }
    }

    protected override void CheckSwitchState()
    {
        if (Methods.CanSeePlayer())
        {
            SwitchState(StateMan.Chase());
        }
        else if (Executor.PatrolPoints.Count > 1)
        {
            SwitchState(StateMan.Patrol());
        }
        else
        {
            
            
        }
    }

    protected override void UpdateState()
    {
        if (Executor.Agent.isActiveAndEnabled)
        {
            if (Executor.Agent.remainingDistance < 1)
            {
                Executor.Animator.ResetTrigger("isPatrolling");
                Executor.Animator.SetTrigger("isIdle");
                Executor.Agent.isStopped = true;
            }
            else
            {
                Executor.Animator.ResetTrigger("isIdle");
                Executor.Animator.SetTrigger("isPatrolling");
                Executor.Agent.isStopped = false;
            }
        }
    }

    protected override void ExitState()
    {
        Executor.Animator.ResetTrigger("isIdle");
        Executor.Animator.ResetTrigger("isPatrolling");
    }

    protected override void InitializeSubStates()
    {
        
    }

}
