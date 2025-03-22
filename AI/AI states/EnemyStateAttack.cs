using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyBaseStates
{
    float _waitTimer;
    Animator _animator;
    bool _hasDamagePlayer;
    public EnemyStateAttack(EnemyStateManager manager, EnemyStateExecutor executor) : base(manager, executor) 
    {
        IsRootState = true;
    }

    public override AIStates CurStateType() { return AIStates.ATTACK; }
    public override void EnterState()
    {
        _animator = Executor.Animator;
        _animator.SetBool("CanAttack", true);
        Executor.Agent.isStopped = true;
        _hasDamagePlayer = false;
    }

    protected override void CheckSwitchState()
    {
        if (!Methods.CanAttackPlayer()
             //wait until attack animation finish. If the animation just finished, wait for a short time
            && !Executor.WaitTimer.IsBetweenOf(0.3f,
               Executor.AttackTime))
        {
            _animator.SetTrigger("Exit");
            //_animator.SetBool("CanAttack", false);
            SwitchState(StateMan.Ground());
        }
    }

    protected override void UpdateState()
    {
        Methods.LookPlayer(2.0f);
        Executor.WaitTimer += Time.deltaTime;
        _waitTimer = Executor.WaitTimer;
        if (Executor.test_showData != null)
        {
            Executor.test_showData.text += "\n" + $"waitTimer: {Executor.WaitTimer}, Attack time: {Executor.AttackTime}, damage Time: {Executor.AttackTime + Executor.PreAtkTime}";
        }
        if (_waitTimer > Executor.AttackTime)
        {
            _hasDamagePlayer = false;
            _animator.ResetTrigger("isIdle");
            _animator.SetTrigger("isMeleeAttacking");
            Executor.chasePlayerForever = false;// stop consider chase player until finish a attack
        }
        if(!_hasDamagePlayer && _waitTimer.IsBetweenOf(
            Executor.AttackTime + Executor.PreAtkTime,
            Executor.AttackTime + Executor.PreAtkTime + 2 * Time.deltaTime ))
        {
            DamageTarget();
        }
        // if the animation finish plays, stop attack and wait for attack CD
        if (_waitTimer > Executor.AttackTime + Executor.AtkAnimTime)
        {
            _hasDamagePlayer = false;
            _animator.ResetTrigger("isMeleeAttacking");
            _animator.SetTrigger("isIdle");
            Executor.WaitTimer = 0;
        }
    }

    void DamageTarget()
    {
        if (Methods.CanDamagePlayer())
        {
            _hasDamagePlayer = true;
            PlayerDmgInfo dmgInfo = new PlayerDmgInfo(Executor.ATK, Executor.Player.position - Executor.transform.position, 250f);
            dmgInfo.CallDamageable(Executor.Player.gameObject);
        }
    }

    protected override void ExitState()
    {
        _animator.ResetTrigger("isIdle");
        _animator.ResetTrigger("isMeleeAttacking");
        _animator.SetBool("CanAttack", false);
        Executor.Agent.enabled = true;
        //Executor.AnimatorEvents.isAttacking = false;
        //Executor.AnimatorEvents.EndAttack();
    }
}

public static class ExpandFloatMethod
{
    public static bool IsBetweenOf(this float x, float min, float max)
    {
        return x >= min && x <= max;
    }
}