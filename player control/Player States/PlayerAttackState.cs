using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    
    public PlayerAttackState(PlayerStatsManager manager, PlayerStateExecutor executor) 
    : base(manager, executor){
        IsRootState = true;
    }

    public override void EnterState()
    {
        Executor.Attacking = true;
        Executor.AttackPressed = false;
        // cannot run and jump during attack
        Executor.CanJump = false;
        Executor.RunPressed = false;
        Executor.CanRun = false;
        Attack();
    }

    protected override void InitializeSubStates()
    {
        if (Executor.MovePressed)
        {
            SetSubState(StateMan.Walk());
        }
        else
        {
            SetSubState(StateMan.Idle());
        }
    }

    protected override void CheckSwitchState()
    {
        if (!Executor.Attacking)
        {
            SwitchState(StateMan.Grounded());
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.ATTACK;
    }

    protected override void UpdateState()
    {
        if(!PlayerBaseMethods.AnimatorIsPlaying(Executor.Animator, "Attack"))
        {
            Executor.Attacking = false;
            Executor.CanJump = true;
            Executor.Animator.speed = 1;
        }
    }

    private void Attack()
    {
        Executor.Animator.CrossFade("Attack", 0.2f);
        Executor.Animator.Play("Attack");
        Executor.Animator.speed *= Executor.GetAtkSpeedMult();
        Executor.AnimEvent.EnableWeaponColl();
        //Executor.SoundMan.PlaySound("Attack");
        Executor.AttackTimer = 0;
        Executor.SoundMan.PlaySound("Attack");
    }


}
