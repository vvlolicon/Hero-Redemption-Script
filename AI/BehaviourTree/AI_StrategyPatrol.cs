using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using OpenCover.Framework.Model;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;

namespace Assets.AI.BehaviourTree
{
    public class AI_StrategyPatrol : IBehaviourSubTree
    {

        private Node _behaviorTree;

        EnemyStateExecutor _executor;
        AIMethods _methods;

        public AI_StrategyPatrol() { }


        public Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            // 创建巡逻行为分支
            _behaviorTree = new SequenceLoop("PatrolLoop");

            // 巡逻移动逻辑
            var patrolAction = new Leaf("PatrolMove", new PatrolMoveStrategy(
                methods, executor
            ));

            _behaviorTree.AddChild(patrolAction);
            return _behaviorTree;
        }
    }

    public class PatrolMoveStrategy : IStrategy
    {
        readonly Transform _aiTransform;
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;

        List<Transform> _patrolPoints;
        Vector3 _currentTarget;
        int _curPatrolIndex;

        public PatrolMoveStrategy (AIMethods methods, EnemyStateExecutor executor)
        {
            _methods = methods;
            _executor = executor;
            _aiTransform = executor.transform;
            _patrolPoints = _executor.PatrolPoints;
            MoveToNextPoint();
        }

        public Node.Status Evaluate()
        {
            _methods.ResetModel();
            if (_methods.CanStartChase() || _executor.IsHit)
            {
                //_executor.Animator.SetBool("IsMoving", false);
                return Node.Status.FAILURE;
            }
            // 检查是否到达目标点
            if (Vector3.Distance(_aiTransform.position, _currentTarget) < 1f )
            {
                if (Vector3.Distance(_aiTransform.position, GetNextPatrolPoint()) > 1f)
                {
                    MoveToNextPoint();
                }
                else
                {
                    _methods.ResetAllAnimationTriggers();
                    _executor.Animator.SetBool("IsIdle", true);
                    return Node.Status.SUCCESS;
                }
            }

            return Node.Status.RUNNING;
        }

        public void OnStatusRunning()
        {
            Debug.Log("Strategy Patrol running");
            _methods.ResetAllAnimationTriggers();
            _executor.Agent.speed = _executor.CombatStats.Speed / 10;
            _executor.Agent.isStopped = false;
            _executor.WaitTimer = 0;
            if (_patrolPoints.Count <= 1)
            {
                MoveToDestination(_patrolPoints[0].position);
                return;
            }
            // find and go back to the closest patrol point
            float lastDist = Mathf.Infinity;
            for (int i = 0; i < _patrolPoints.Count; i++)
            {
                Vector3 thisWP = _patrolPoints[i].position;
                float distance = Vector3.Distance(_executor.transform.position, thisWP);
                //Debug.Log($"distance to patrol point {i} is {distance}");
                if (distance < lastDist)
                {
                    _curPatrolIndex = i;
                    lastDist = distance;
                }
            }
            //Debug.Log($"move to patrol point {_curPatrolIndex}");
            MoveToDestination(_patrolPoints[_curPatrolIndex].position);
        }

        public void OnStatusFailure()
        {
            _executor.Agent.ResetPath();
            _executor.Animator.SetBool("IsIdle", false);
            _executor.Animator.SetBool("IsPatrolling", false);
        }

        Vector3 GetNextPatrolPoint()
        {
            // 如果只有一个巡逻点，直接返回该点
            if (_patrolPoints.Count == 1)
            {
                return _patrolPoints[0].position;
            }
            _curPatrolIndex = (_curPatrolIndex + 1) % _patrolPoints.Count;
            Vector3 nextpoint = _patrolPoints[_curPatrolIndex].position;
            //Debug.Log($"next patrol point index: {_curPatrolIndex}");
            return nextpoint;
        }
        void MoveToNextPoint()
        {
            MoveToDestination(GetNextPatrolPoint());
            //_executor.Animator.SetBool("IsMoving", true);

        }

        void MoveToDestination(Vector3 goal)
        {
            _currentTarget = goal;
            _executor.Agent.ResetPath();
            _executor.Agent.SetDestination(_currentTarget);
            _methods.ResetAllAnimationTriggers();
            _executor.Animator.SetBool("IsPatrolling", true);
            //Debug.Log($"moving to {_currentTarget.x}, {_currentTarget.y}, {_currentTarget.z}");
        }

        public void Reset()
        {
            //Debug.Log("reset Strategy status");
        }
    }
}