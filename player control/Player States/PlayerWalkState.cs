using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStatsManager manager, PlayerStateExecutor executor)
    : base(manager, executor) {}

    public override PlayerStates CurStateType()
    {
        return PlayerStates.WALK;
    }

    protected override void CheckSwitchState()
    {
        if (!Executor.MovePressed)
        {
            SwitchState(StateMan.Idle());
        }
    }

    public override void EnterState(){
        Executor.Animator.SetBool("Walk", true);
    }

    protected override void ExitState()
    {
        Executor.Animator.SetBool("Walk", false);
        Executor.Animator.SetBool("Run", false);
        //Executor.Animator.SetTrigger("Exit");
    }

    protected override void UpdateState()
    {
        Executor.Animator.SetBool("Walk", true);
        //Executor.Animator.SetFloat("Speed", Executor.InputMoveXZ.magnitude);

        if (Executor.RunPressed)
        {
            Executor.Animator.SetBool("Run", true);
        }
        else
        {
            Executor.Animator.SetBool("Run", false);
        }
        if (Executor.Attacking)
        {
            Executor.CurPlayerSpeed = Executor.PlayerSpeed * Executor.AtkSpeedMult;
        }
        else if (Executor.RunPressed)
        {
            Executor.CurPlayerSpeed = Executor.PlayerSpeed * Executor.RunSpeedMult;
        }
        else
        {
            Executor.CurPlayerSpeed = Executor.PlayerSpeed;
        }
        //Executor.Animator.SetFloat("Speed", Executor.CurPlayerSpeed);
        PlayerMethods.CalculateMovement(Executor.CurPlayerSpeed, Executor.MovementY);
    }
    
}
