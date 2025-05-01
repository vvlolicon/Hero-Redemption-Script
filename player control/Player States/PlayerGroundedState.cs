
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
        else if (!Executor.CharCont.isGrounded && PlayerMethods.DistToGround() > 0.5f)
        {
            //Debug.Log("Ground state switch to fall state, " + "isGrounded: " + Executor.CharCont.isGrounded + ", check Grounded: " + PlayerMethods.IsGrounded());
            SwitchState(StateMan.Fall());
            Executor.Animator.Play("Falling");// from ground directly to fall
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
        //Executor.Animator.SetBool("Jump", false);
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
