
using UnityEngine;

public class PlayerHitState : PlayerBaseState
{
    float hitTimer;
    public PlayerHitState(PlayerStatsManager manager, PlayerStateExecutor executor)
    : base(manager, executor) {
        IsRootState = true;
    }

    protected override void CheckSwitchState()
    {
        if (!Executor.IsHit)
        {
            if (Executor.CharCont.isGrounded)
            {
                SwitchState(StateMan.Grounded());
            }
            else
            {
                SwitchState(StateMan.Fall());
            }
        }
    }
    public override PlayerStates CurStateType()
    {
        return PlayerStates.HIT;
    }

    public override void EnterState()
    {
        Executor.IsHit = true;
        Executor.CanMove = false;
        Executor.CanJump = false;
        Executor.CanRun = false;
        Executor.RunPressed = false;
        Executor.Attacking = false;
        Executor.Animator.SetTrigger("Exit");
        hitTimer = 0f;
    }
    protected override void ExitState()
    {
        Executor.CanMove = true;
        Executor.CanJump = true;
        Executor.CanRun = true;
    }
    protected override void UpdateState()
    {
        HitUpdate();
    }

    void HitUpdate()
    {
        hitTimer += Time.deltaTime;
        Vector3 moveDirection = Executor.CurMovement;
        Vector3 impact = Executor.Impact;
        moveDirection.y -= Executor.Gravity * Time.deltaTime;
        Vector3 impactGrav = new Vector3(impact.x, impact.y + moveDirection.y, impact.z); //Adds gravity to the impact force
        // apply the impact force:
        if (impact.magnitude > 0.2f || !Executor.CharCont.isGrounded) 
            Executor.CharCont.Move(impactGrav * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        if (Executor.CharCont.isGrounded &&
            //impact.magnitude <= 0.2f)
            hitTimer > 1.3f)
        {
            Executor.IsHit = false;
            //Executor.Animator.SetTrigger("Exit");
            //Executor.Animator.Play("Idle"); //To unlock the animation in some weird cases
        }
        Executor.CurMovement = moveDirection;
        Executor.Impact = impact;
    }

    public void AddImpact(Vector3 dir, float force) //Apply a force to the player
    {
        Executor.CurMovement = Vector3.zero;
        Executor.Animator.Play("Hit");
        Executor.SoundMan.PlaySound("Hurt");

        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; //Reflect down force on the ground
            Executor.Impact += dir.normalized * force / Executor.Mass;
    }

    public void ApplyDamage(PlayerDmgInfo info) //Apply damage to the player
    {
        //Executor.SoundMan.PlaySound("Hit");
        //currentDashTime = maxDashTime; //Cancels dash if was pressed
        Executor.Animator.SetFloat("Speed", 0);
        Executor.Animator.StopPlayback();
        Executor.Animator.GetComponent<AnimatorEvents>().DisableWeaponColl();
        AddImpact(info.DmgDir, info.Force);
    }
}
