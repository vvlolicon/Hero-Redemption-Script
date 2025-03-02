using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyBaseStates
{
    float _waitTimer;
    Animator _animator;
    public EnemyStateAttack(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor) 
    {
        IsRootState = true;
    }

    public override AIStates CurStateType() { return AIStates.ATTACK; }
    public override void EnterState()
    {
        _animator = Executor.Animator;
        _animator.SetTrigger("isIdle");
        Executor.Agent.isStopped = true;
    }

    protected override void CheckSwitchState()
    {
        if (!Methods.CanAttackPlayer()
             //wait for a little bit second before start chasing player
            && Executor.WaitTimer > Executor.AtkAnimTime + 0.3f)
        {
            SwitchState(StateMan.Ground());
        }
    }

    protected override void UpdateState()
    {
        Methods.LookPlayer(2.0f);

        Executor.WaitTimer += Time.deltaTime;
        _waitTimer = Executor.WaitTimer;
        //Executor.test_showState.text += "\n"+ "waitTimer: " + _waitTimer + ", Attack time:" + Executor.AttackTime + ", ";

        if (_waitTimer > Executor.AttackTime)
        {
            Executor.WaitTimer = 0;
            _animator.ResetTrigger("isIdle");
            _animator.SetTrigger("isMeleeAttacking");
            Executor.AnimatorEvents.isAttacking = true;
        }
        // if the animation finish plays, stop attack and wait for attack CD
        else if (_waitTimer > Executor.AtkAnimTime)
        {
            _animator.ResetTrigger("isMeleeAttacking");
            _animator.SetTrigger("isIdle");
        }
    }

    protected override void ExitState()
    {
        _animator.ResetTrigger("isIdle");
        _animator.ResetTrigger("isMeleeAttacking");
        Executor.AnimatorEvents.isAttacking = false;
        Executor.Agent.isStopped = false;
        Executor.AnimatorEvents.EndAttack();
    }
}
