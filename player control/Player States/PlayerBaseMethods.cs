using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerBaseMethods
{
    PlayerStateExecutor _executor;

    public PlayerBaseMethods(PlayerStateExecutor executor)
    {
        _executor = executor;
    }

    public void CalculateMovement(float speed, float directionY)
    {
        Vector3 moveDirection = new Vector3(_executor.InputMoveXZ.x, 0, _executor.InputMoveXZ.y);
        if (moveDirection.magnitude < 0.1f) //Because if velocity is minor than 0.1 the animator dont play the correct animation
            moveDirection = Vector3.zero;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1); //Limits the vector magnitude to 1
        _executor.MovIndicator.transform.localPosition = moveDirection;
        _executor.Animator.SetFloat("Speed", moveDirection.magnitude);
        if (moveDirection.magnitude > 0) //Fixes the problem when there is no movement
        {
            //To rotate the controller when moving and position it correctly relative to the camera
            _executor.CharCont.transform.rotation = new Quaternion(_executor.CharCont.transform.rotation.x, _executor.Camera.transform.rotation.y, _executor.CharCont.transform.rotation.z, _executor.Camera.transform.rotation.w);

            //Smoothly rotate the character in the xz plane towards the direction of movement
            Vector3 targetActPosition = new Vector3(_executor.MovIndicator.transform.position.x, _executor.ChildPlayer.transform.position.y, _executor.MovIndicator.transform.position.z);
            Quaternion rotation = Quaternion.LookRotation(targetActPosition - _executor.ChildPlayer.transform.position);
            _executor.ChildPlayer.transform.rotation = Quaternion.Slerp(_executor.ChildPlayer.transform.rotation, rotation, Time.deltaTime * 10);
        }
        //Rotate it to the player orientation
        moveDirection = _executor.gameObject.transform.TransformDirection(moveDirection);
        moveDirection = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed); // apply the horizontal speed
        if (!IsGrounded() && _executor.CurState.CurStateType() == PlayerStates.GROUNDED)
            moveDirection.y = -10f; //To prevent the controller from taking off when going down ramps
        else
            moveDirection.y = directionY;

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= _executor.Gravity * Time.deltaTime;
        _executor.CurMovement = moveDirection;
    }

    public bool IsGrounded() //Check if ground is under the character
    {
        return Physics.Raycast(_executor.gameObject.transform.position, -Vector3.up, _executor.DistToGround + 0.1f);
    }

    public void HandleFalling()
    {
        _executor.Animator.SetFloat("SpeedY", _executor.CharCont.velocity.y);
        // falling speed should not exceed the gravity
        _executor.MovementY = Mathf.Max(_executor.CurMovement.y, -1 * _executor.Gravity);
        //executor.MovementY -= executor.Gravity * Time.deltaTime;
        if (_executor.CharCont.isGrounded && !_executor.WasGrounded)
        {
            _executor.CanJump = true;
        }
        if (_executor.WasGrounded && _executor.CanJump) //If it is the first frame of falling
        {
            if (DistToGround() > 0.5f) //If the ground is far enough
            {
                _executor.MovementY = 0f;
                _executor.WasGrounded = false;
                _executor.Animator.SetBool("Jump", true); //Needed for activate the jump state if character falls
                _executor.Animator.CrossFade("Falling", 0.2f);
            }
        }
        if (_executor.CharCont.velocity.y < 0) //If player is falling down
            _executor.FallTime += Time.deltaTime;
        _executor.WasGrounded = _executor.CharCont.isGrounded;
    }

    float DistToGround() //Calculates the distance to the ground when starts to fall
    {
        RaycastHit hit;
        if (Physics.Raycast(_executor.gameObject.transform.position, -Vector3.up, out hit, _executor.DistToGround + 999))
            return hit.distance - _executor.DistToGround;
        else return 999;
    }

    public static bool AnimatorIsPlaying(Animator animator, string stateName)
    {
        bool isAnimationPlaying = animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return isAnimationPlaying && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

}
