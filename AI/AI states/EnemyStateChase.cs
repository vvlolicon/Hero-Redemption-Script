using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyBaseStates
{
    public EnemyStateChase(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor) { }

    public override AIStates CurStateType() { return AIStates.CHASE; }
    public override void EnterState()
    {
        Executor.Agent.speed = Executor.Speed * 1.5f; // change to running speed
        Executor.Agent.isStopped = false;
        Executor.Animator.SetTrigger("isChasing");
    }

    protected override void CheckSwitchState()
    {
        if (Executor.Agent.hasPath)
        {
            if (Methods.CanAttackPlayer())
            {
                SwitchState(StateMan.Attack());
                Executor.Agent.isStopped = true;
            }
            else if (Methods.CanStopChase())
            {
                if (Executor.PatrolPoints.Count > 0)
                {
                    SwitchState(StateMan.Patrol());
                }
                else
                {
                    SwitchState(StateMan.Idle());
                }
            }
        } 
        else
        {
            if (Executor.PatrolPoints.Count > 0)
            {
                SwitchState(StateMan.Patrol());
            }
            else
            {
                SwitchState(StateMan.Idle());
            }
        }
    }

    protected override void UpdateState()
    {
        if(Executor.WaitTimer < Executor.AttackTime)
        {
            Executor.WaitTimer += Time.deltaTime;
        }
        Executor.Agent.SetDestination(Executor.Player.position);
        //Executor.test_showState.text += "\n" + "player pos: " + Executor.Player.position + "\n" + "Agent has Path: " + Executor.Agent.hasPath;
    }

    protected override void ExitState()
    {
        Executor.Agent.speed = Executor.Speed; // recover to default speed
        Executor.Animator.ResetTrigger("isChasing");
    }
}
