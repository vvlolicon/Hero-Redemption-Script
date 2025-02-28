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
        Executor.SoundManager.PlaySound("Hit");
        Executor.Animator.SetTrigger("isHited");
        Executor.Agent.isStopped = true;
        waitTimer = 0;
    }

    protected override void CheckSwitchState()
    {
        if (waitTimer >= 1.25f)
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
        Executor.Animator.ResetTrigger("isHited");
        waitTimer = 0;
    }
}
