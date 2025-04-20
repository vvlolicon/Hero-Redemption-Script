using System;
using System.Collections;
using UnityEngine;

namespace Assets.AI.BehaviourTree
{

    public class AI_MeleeAttack : AI_AttackStrategyBase, IBehaviourSubTree
    {
        public Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            _behaviorTree = new Sequence("ST_Attack");
            _behaviorTree.AddChild(new Leaf("CanAttackPlayer",
                new Condition(() => { return _methods.CanAttackPlayer(); })));
            _behaviorTree.AddChild(new Leaf("LA_Attack Player", new Strategy_AI_MeleeAttack(methods, executor)));
            return _behaviorTree;
        }
    }

    public class Strategy_AI_MeleeAttack : IStrategy
    {
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;
        float _waitTimer;
        bool _hasDamagePlayer;

        Animator _animator { get { return _executor.Animator; } }

        public Strategy_AI_MeleeAttack(
            AIMethods methods,
            EnemyStateExecutor executor)
        {
            _methods = methods;
            _executor = executor;
        }
        public Node.Status Evaluate()
        {
            if (!_methods.CanAttackPlayer())
            {
                Reset();
                return Node.Status.FAILURE;
            }
            _methods.LookPlayer(2.0f);
            _executor.WaitTimer += Time.deltaTime;
            _waitTimer = _executor.WaitTimer;

            if (_waitTimer > _executor.AttackTime)
            {
                _hasDamagePlayer = false;
                _animator.SetTrigger("IsMeleeAttacking");
                _executor.chasePlayerForever = false;// stop consider chase player until finish a attack
            }
            if (!_hasDamagePlayer && _waitTimer.IsBetweenOf(
                _executor.AttackTime + _executor.PreAtkTime,
                _executor.AttackTime + _executor.PreAtkTime + 2 * Time.deltaTime))
            {
                DamageTarget();
            }
            // if the animation finish plays, stop attack and wait for attack CD
            if (_waitTimer > _executor.AttackTime + _executor.AtkAnimTime)
            {
                _methods.ResetModel();
                _hasDamagePlayer = false;
                _animator.ResetTrigger("IsMeleeAttacking");
                _executor.WaitTimer = 0;
            }
            return Node.Status.RUNNING;

            void DamageTarget()
            {
                if (_methods.CanDamagePlayer())
                {
                    _hasDamagePlayer = true;
                    PlayerDmgInfo dmgInfo = new PlayerDmgInfo(
                        _executor.ATK,
                        _executor.Player.position - _executor.transform.position,
                        250f);
                    dmgInfo.CallDamageable(_executor.Player.gameObject);
                }
            }
        }
        public void OnStatusRunning()
        {
            _methods.ResetAllAnimationTriggers();
            _animator.SetBool("CanAttack", true);
            _executor.Agent.isStopped = true;
            _hasDamagePlayer = false;
        }
        public void OnStatusFailure()
        {
            _animator.ResetTrigger("IsMeleeAttacking");
            _animator.SetBool("CanAttack", false);
            _executor.Agent.isStopped = false;
        }

        public void Reset()
        {
            _waitTimer = 0f;
            _hasDamagePlayer = false;
            //Debug.Log("reset Strategy status");
        }

    }
}