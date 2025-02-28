using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStatsManager manager, PlayerStateExecutor executor)
    : base(manager, executor) {
        IsRootState = true;
        
    }

    protected override void CheckSwitchState()
    {
        if (Executor.JumpPressed && Executor.CanJump)
        {
            SwitchState(StateMan.Jump());
        }
        else if (!Executor.CharCont.isGrounded)
        {
            //Debug.Log("Ground state switch to fall state, " + "isGrounded: " + Executor.CharCont.isGrounded + ", check Grounded: " + PlayerMethods.IsGrounded());
            SwitchState(StateMan.Fall());
        }
        else if (Executor.AttackPressed)
        {
            SwitchState(StateMan.Attack());
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.GROUNDED;
    }

    public override void EnterState()
    {
        if (SubState == null)
            InitializeSubStates();
        Executor.CanRun = true;
        Executor.CanMove = true;
        Executor.CanJump = true;
        Executor.MovementY = Executor.GroundedGravity;
        Executor.Animator.SetBool("Jump", false);
        if (Executor.FallTime > 0.2f)
        {
            Executor.SoundMan.PlaySound("Land");
            Executor.Animator.SetBool("Grounded", true);
            //if (!hit)
            //Executor.Animator.CrossFade("FallingEnd", 0.1f);
        }
        Executor.FallTime = 0f;
        Executor.WasGrounded = Executor.CharCont.isGrounded;
    }

    protected override void InitializeSubStates()
    {
        if (Executor.MovePressed)
        {
            SetSubState(StateMan.Walk());
        }
        else if (!Executor.JumpPressed && !Executor.RunPressed && !Executor.AttackPressed)
        {
            SetSubState(StateMan.Idle());
        }
    }

    protected override void UpdateState()
    {
        
    }
}
