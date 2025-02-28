using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStatsManager manager, PlayerStateExecutor executor)
     : base(manager, executor)
    {
        IsRootState = true;
    }

    protected override void CheckSwitchState()
    {
        if (!Executor.CharCont.isGrounded)
        {
            //Debug.Log("Jump state switch to fall state, " + "isGrounded: " + Executor.CharCont.isGrounded + ", check Grounded: " + PlayerMethods.IsGrounded());
            SwitchState(StateMan.Fall());
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.JUMP;
    }

    public override void EnterState()
    {
        // cannot switch to running during jumping. but can keep running
        Executor.CanRun = false;
        Executor.JumpPressed = false;
        Executor.CanJump = false;
        Executor.MovementY = Executor.JumpSpeed;
        Executor.Animator.SetFloat("SpeedY", Executor.MovementY);
        Executor.Animator.Play("Falling");
        Executor.Animator.SetBool("Jump", true);
    }

    protected override void UpdateState()
    {
        PlayerMethods.HandleFalling();
    }
    
}