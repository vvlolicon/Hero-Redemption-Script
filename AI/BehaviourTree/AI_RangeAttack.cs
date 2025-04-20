using System;
using System.Collections;
using UnityEngine;

namespace Assets.AI.BehaviourTree
{

    public class AI_RangeAttack : MonoBehaviour, IBehaviourSubTree
    {
        Node _behaviorTree;

        EnemyStateExecutor _executor;
        AIMethods _methods;
        public ProjectileStats _projectileStats;
        public Transform _firePosition;
        Transform _aimPosition;
        public Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            _behaviorTree = new Sequence("ST_Attack");
            _behaviorTree.AddChild(new Leaf("CanAttackPlayer",
                new Condition(() => { return _methods.CanAttackPlayer(); })));
            _behaviorTree.AddChild(new Leaf("LA_Attack Player", 
                new Strategy_AI_RangeAttack(
                    methods, executor, _projectileStats, FireProjectile
            )));
            _aimPosition = executor.Player.GetComponent<PlayerStateExecutor>()._rangeAtkAim;
            return _behaviorTree;
        }

        public void FireProjectile()
        {
            GameObject projectile = Instantiate(
                _projectileStats.ProjectilePrefab, _firePosition.position,
                Quaternion.FromToRotation(transform.position, _aimPosition.position)
            );
            ProjectileControl contrScript = projectile.AddComponent<ProjectileControl>();
            if (_projectileStats.useGravity == true && _projectileStats.IsAOE == true)
            {// projectile like a granade, throw to the ground of the character 
                contrScript.SetStats(_executor, _projectileStats, _executor.Player.position);
            }
            else
            {
                contrScript.SetStats(_executor, _projectileStats, _aimPosition.position);
            }
        }
    }

    public class Strategy_AI_RangeAttack : IStrategy
    {
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;
        readonly ProjectileStats _attackProjectile;
        float _waitTimer;
        bool _firedProjectile = false;

        event Action _actionFireProjectile;

        Animator _animator { get { return _executor.Animator; } }

        public Strategy_AI_RangeAttack(
            AIMethods methods,
            EnemyStateExecutor executor,
            ProjectileStats attackProjectile,
             Action actionFireProjectile
            )
        {
            _methods = methods;
            _executor = executor;
            _attackProjectile = attackProjectile;
            _actionFireProjectile = actionFireProjectile;
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
                _animator.SetTrigger("IsMeleeAttacking");
                _executor.chasePlayerForever = false;// stop consider chase player until finish a attack
            }
            // if the animation finish plays, fire projectile and wait for attack CD
            if (_waitTimer > _executor.AttackTime + _executor.AtkAnimTime)
            {
                _actionFireProjectile();
                _methods.ResetModel();
                _animator.ResetTrigger("IsMeleeAttacking");
                _executor.WaitTimer = 0;
            }
            return Node.Status.RUNNING;

            
        }
        public void OnStatusRunning()
        {
            _methods.ResetAllAnimationTriggers();
            _animator.SetBool("CanAttack", true);
            _executor.Agent.isStopped = true;
            _firedProjectile = false;
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
            _firedProjectile = false;
            //Debug.Log("reset Strategy status");
        }

    }
}