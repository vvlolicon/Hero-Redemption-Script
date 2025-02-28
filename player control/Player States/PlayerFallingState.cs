using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFallingState : PlayerBaseState
{
    public PlayerFallingState(PlayerStatsManager manager, PlayerStateExecutor executor)
    : base(manager, executor) {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Executor.CanRun = false;
        Executor.CanJump = false;
        Executor.JumpPressed = false;
    }
    protected override void CheckSwitchState()
    {
        if (Executor.CharCont.isGrounded)
        {
            //Debug.Log("Fall state switch to ground state, " + "isGrounded: " + Executor.CharCont.isGrounded + ", check Grounded: " + PlayerMethods.IsGrounded());
            SwitchState(StateMan.Grounded());
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.FALL;
    }

    protected override void ExitState()
    {
        //Executor.Animator.Play("Landing");
    }

    protected override void UpdateState()
    {
        PlayerMethods.HandleFalling();
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

    
}
