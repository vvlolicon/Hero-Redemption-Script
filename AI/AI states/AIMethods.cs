using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMethods
{
    Transform _enemy;
    EnemyStateExecutor _executor;
    Transform _player;
    float _visDist;
    float _visAngle;
    float _attackDist;
    Animator _animator;

    public AIMethods(EnemyStateExecutor executor) { 
        _executor = executor;
        _enemy = _executor.transform;
        _player = _executor.Player;
        _visDist = _executor.VisDist;
        _visAngle = _executor.VisAngle;
        _attackDist = _executor.AttackDist;
        _animator = _executor.Animator;
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = _player.position - _enemy.position;
        float angle = Vector3.Angle(direction, _enemy.forward);

        if (direction.magnitude < _visDist && angle < _visAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        return IsPlayerInRange(_attackDist);
    }

    public bool CanDamagePlayer()
    {
        return IsPlayerInRange(_attackDist * 2);
    }

    public bool IsPlayerInRange(float range)
    {
        Vector3 direction = _player.position - _enemy.position;
        if (direction.magnitude < range)
        {
            return true;
        }
        return false;
    }

    public bool IsPlayerTooFar() //Stop follow the player
    {
        Vector3 direction = _player.position - _enemy.position;
        if (direction.magnitude > _visDist)
        {
            return true;
        }
        return false;
    }
    public bool CanStopChase()
    {
        return !_executor.chasePlayerForever && IsPlayerTooFar();
    }
    public bool CanStartChase()
    {
        return _executor.chasePlayerForever || CanSeePlayer();
    }
    public void LookPlayer(float speedRot)
    {
        Vector3 direction = _player.position - _enemy.position;
        float angle = Vector3.Angle(direction, _enemy.forward);
        direction.y = 0;

        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.Slerp(_enemy.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speedRot);
    }
    public bool IsInterrupteByAttack()
    {
        return _executor.IsHit && _executor.Interruptable;
    }
    public bool CanStopAttack()
    {
        return !CanAttackPlayer() || IsInterrupteByAttack();
    }

    public bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public void ResetModel()
    {
        Transform model = _executor.Animator.transform;
        model.localPosition = Vector3.zero;
        model.localRotation = Quaternion.Euler(Vector3.zero);
    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ResetAllAnimationTriggers()
    {
        _animator.SetBool("IsIdle", false);
        _animator.SetBool("IsChasing", false);
        _animator.SetBool("IsPatrolling", false);
        _animator.SetBool("CanAttack", false);
        _animator.ResetTrigger("IsMeleeAttacking");
        _animator.ResetTrigger("IsHit");
        _animator.ResetTrigger("Exit");
    }

}
