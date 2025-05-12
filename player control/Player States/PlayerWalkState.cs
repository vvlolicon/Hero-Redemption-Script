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
        walkTimer = (Executor.RunPressed)? runTimeInterval : walkTimeInterval;
        if (Executor.CurState.CurStateType() == PlayerStates.FALL) return;
        // do not play sound if is not on ground
        Executor.SoundMan.PlaySound("Walk");
        if (Executor.RunPressed)
        {
            Executor.SoundMan.PlaySound("Run");
        }
        else
        {
            Executor.SoundMan.PlaySound("Walk");
        }
    }

    protected override void ExitState()
    {
        Executor.Animator.SetBool("Walk", false);
        Executor.Animator.SetBool("Run", false);
        //Executor.Animator.SetTrigger("Exit");
    }

    float walkTimer = 0f;
    float walkTimeInterval = 1f;
    float runTimeInterval = 0.3f;
    protected override void UpdateState()
    {
        Executor.Animator.SetBool("Walk", true);
        walkTimer -= Time.deltaTime;
        bool playSound = walkTimer <= 0f &&
            Executor.CurState.CurStateType() != PlayerStates.FALL;
        if (Executor.RunPressed)
        {
            Executor.Animator.SetBool("Run", true);
            if (playSound)
            {
                Executor.SoundMan.PlaySound("Run");
                walkTimer = runTimeInterval;
            }
        }
        else
        {
            Executor.Animator.SetBool("Run", false);
            if (playSound)
            {
                Executor.SoundMan.PlaySound("Walk");
                walkTimer = walkTimeInterval;
            }
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
