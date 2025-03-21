using GLTF.Schema;
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
        Vector3 direction = _player.position - _enemy.position;
        if (direction.magnitude < _attackDist)
        {
            return true;
        }
        return false;
    }

    public bool CanDamagePlayer()
    {
        Vector3 direction = _player.position - _enemy.position;
        if (direction.magnitude < _attackDist * 2)
        {
            return true;
        }
        return false;
    }

    public bool CanStopChase() //Stop follow the player
    {
        Vector3 direction = _player.position - _enemy.position;
        if (direction.magnitude > _visDist)
        {
            return true;
        }
        return false;
    }
    public void LookPlayer(float speedRot)
    {
        Vector3 direction = _player.position - _enemy.position;
        float angle = Vector3.Angle(direction, _enemy.forward);
        direction.y = 0;

        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.Slerp(_enemy.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speedRot);
    }

    public bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

}
