using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyBaseStates
{
    float chaseTimer;
    public EnemyStateChase(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor) { }

    public override AIStates CurStateType() { return AIStates.CHASE; }
    public override void EnterState()
    {
        Executor.Agent.speed = Executor.Speed * 2f; // change to running speed
        Executor.Agent.isStopped = false;
        Executor.Animator.SetTrigger("isChasing");
        chaseTimer = 0;
        //Executor.Animator.SetTrigger("Walking");
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
            else if (!Executor.chasePlayerForever)
            {
                if (Methods.IsPlayerTooFar() || chaseTimer > Executor.ChaseTime)
                {
                    if (Executor.PatrolPoints.Count > 1)
                    {
                        SwitchState(StateMan.Patrol());
                    }
                    else
                    {
                        SwitchState(StateMan.Idle());
                    }
                }
            }
        } 
        else
        {
            if (Executor.PatrolPoints.Count > 1)
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
        chaseTimer += Time.deltaTime;
        Executor.Animator.SetTrigger("isChasing");
        if (Executor.WaitTimer < Executor.AttackTime)
        {
            Executor.WaitTimer += Time.deltaTime;
        }
        Executor.Agent.SetDestination(Executor.Player.position);
        //Executor.test_showState.text += "\n" + "player pos: " + Executor.Player.position + "\n" + "Agent has Path: " + Executor.Agent.hasPath;
    }

    protected override void ExitState()
    {
        chaseTimer = 0;
        Executor.Agent.speed = Executor.Speed; // recover to default speed
        Executor.Animator.ResetTrigger("isChasing");
    }
}
