using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol : EnemyBaseStates
{
    public int curPatrolIndex = -1;//The point of the patrol points where the enemy goes
    public EnemyStatePatrol(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor) { }

    public override AIStates CurStateType() { return AIStates.PATROL; }
    public override void EnterState()
    {
        Executor.Agent.speed = Executor.Speed;
        Executor.Agent.isStopped = false;
        Executor.WaitTimer = 0;
        // find and go back to the closest patrol point
        float lastDist = Mathf.Infinity;
        for (int i = 0; i < Executor.PatrolPoints.Count; i++)
        {
            GameObject thisWP = Executor.PatrolPoints[i];
            float distance = Vector3.Distance(Executor.transform.position, thisWP.transform.position);
            if (distance < lastDist)
            {
                curPatrolIndex = i - 1; //Because in the update it will be added one
                lastDist = distance;
            }
        }
        Executor.Animator.SetTrigger("isPatrolling");
    }

    protected override void CheckSwitchState()
    {
        if (Methods.CanSeePlayer())
        {
            SwitchState(StateMan.Chase());
        }
    }

    protected override void UpdateState()
    {
        if (Executor.Agent.remainingDistance < 1)
        {
            if (curPatrolIndex >= Executor.PatrolPoints.Count - 1)
                curPatrolIndex = 0;
            else
                curPatrolIndex++;
            Executor.Agent.SetDestination(Executor.PatrolPoints[curPatrolIndex].transform.position);
        }
    }

    protected override void ExitState()
    {
        Executor.Animator.ResetTrigger("isPatrolling");
    }
}
