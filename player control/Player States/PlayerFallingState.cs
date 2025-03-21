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
        Executor.FallTime = 0f;
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
        
        if (Executor.FallTime > 0.2f)
        {
            //Executor.Animator.SetBool("Grounded", true);
        }
        if (Executor.FallTime > 0.5f)
        {
            Executor.SoundMan.PlaySound("Land");
            Executor.OnLanding();
            Executor.Animator.Play("Landing");
        }
        else
        {
            Executor.Animator.SetTrigger("Exit");
        }
        Executor.FallTime = 0f;
        Executor.WasGrounded = Executor.CharCont.isGrounded;
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
