using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateHit : EnemyBaseStates
{
    float waitTimer;
    public EnemyStateHit(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor){
        IsRootState = true;
    }

    public override AIStates CurStateType() { return AIStates.HIT; }
    public override void EnterState()
    {
        //Executor.SoundManager.PlaySound("Hit");
        Executor.Animator.SetTrigger("IsHit");
        Executor.Animator.Play("Hit");
        Executor.Animator.SetBool("CanAttack", false);
        Executor.Animator.ResetTrigger("isIdle");
        Executor.Animator.ResetTrigger("isPatrolling");
        Executor.Animator.ResetTrigger("isMeleeAttacking");
        Executor.Animator.ResetTrigger("isChasing");
        Executor.Animator.ResetTrigger("isPatrolling");
        Executor.Animator.SetTrigger("Exit");
        Executor.chasePlayerForever = true;
        if (Executor.Agent.isActiveAndEnabled)
            Executor.Agent.isStopped = true;
        waitTimer = 0;
        if(Executor.AnimatorEvents!= null)
            Executor.AnimatorEvents.EndAttack();
    }

    protected override void CheckSwitchState()
    {
        if (waitTimer >= Executor.HitAnimTime)
        {
            if (Methods.CanAttackPlayer())
                SwitchState(StateMan.Attack());
            else
                SwitchState(StateMan.Ground());
        }
    }

    protected override void UpdateState()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer < 0.5f)
        {
            Methods.LookPlayer(5.0f);
        }
        else if (waitTimer >= 0.5f && Executor.IsInvincible)
        {
            Executor.IsInvincible = false;
        }
    }
    protected override void ExitState()
    {
        Executor.Animator.ResetTrigger("IsHit");
        waitTimer = 0;
    }
}


