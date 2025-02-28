using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStatsManager manager, PlayerStateExecutor executor)
    : base(manager, executor) {}

    protected override void CheckSwitchState()
    {
        if (Executor.MovePressed)
        {
            //Debug.Log("triggered Switch move state");
            SwitchState(StateMan.Walk());
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.IDLE;
    }

    public override void EnterState()
    {
        Executor.CurMovement = new Vector3(0, Executor.CurMovement.y, 0);
        Executor.Animator.SetFloat("Speed", 0);
        Executor.RunPressed = false;
    }

    protected override void UpdateState()
    {
        Executor.CurPlayerSpeed = Executor.PlayerSpeed;
        PlayerMethods.CalculateMovement(Executor.CurPlayerSpeed, Executor.MovementY);
    }
}
