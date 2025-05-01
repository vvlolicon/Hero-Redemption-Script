
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    float idleTime;
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

    protected override void ExitState()
    {
        idleTime = 0;
        Executor.Animator.SetTrigger("Exit");
    }

    public override void EnterState()
    {
        Executor.CurMovement = new Vector3(0, Executor.CurMovement.y, 0);
        //Executor.Animator.SetFloat("Speed", 0);
        Executor.RunPressed = false;
        idleTime = 0;
    }

    protected override void UpdateState()
    {
        Executor.CurPlayerSpeed = Executor.PlayerSpeed;
        PlayerMethods.CalculateMovement(Executor.CurPlayerSpeed, Executor.MovementY);
        idleTime += Time.deltaTime;
        if(idleTime > 5f)
        {
            Executor.Animator.Play("LookAround");
            idleTime = -10f;
        }
    }
}
