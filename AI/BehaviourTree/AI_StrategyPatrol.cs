using UnityEngine;
using System.Collections.Generic;

namespace Assets.AI.BehaviourTree
{
    public class AI_StrategyPatrol : IBehaviourSubTree
    {

        private Node _behaviorTree;
        private PatrolMoveStrategy _currentStrategy;
        EnemyStateExecutor _executor;
        AIMethods _methods;

        public AI_StrategyPatrol() { }


        public Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            if (_currentStrategy != null)
            { // deregister restart event for previous strategy
                _executor.OnRestartPatrolRequested -= _currentStrategy.HandleRestartRequest;
            }
            _executor = executor;
            _methods = methods;
            _currentStrategy = new PatrolMoveStrategy(methods, executor);
            // register restart event for executor to execute strategy again
            _executor.OnRestartPatrolRequested += _currentStrategy.HandleRestartRequest;
            // looping forever to patrol
            _behaviorTree = new SequenceLoop("PatrolLoop");
            var patrolAction = new Leaf("PatrolMove", _currentStrategy);

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

        float idleTimer;
        float idleSoundInterval = 10f;
        float walkTimer = 1f;
        float walkTimeInterval = 1f;

        public PatrolMoveStrategy (AIMethods methods, EnemyStateExecutor executor)
        {
            _methods = methods;
            _executor = executor;
            _aiTransform = executor.transform;
            _patrolPoints = _executor.PatrolPoints;
            _executor.OnRestartPatrolRequested += HandleRestartRequest;
            idleTimer = RandomIdleSoundInterval();
            MoveToNextPoint();
        }

        float RandomIdleSoundInterval()
        {
            return idleSoundInterval + Random.Range(-2f, 2f);
        }

        public Node.Status Evaluate()
        {
            _methods.ResetModel();
            if (_methods.CanStartChase() || _executor.IsHit)
            {
                //_executor.Animator.SetBool("IsMoving", false);
                return Node.Status.FAILURE;
            }
            // play sound if not failure
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                _executor.SoundManager.PlayExtraSound("Idle");
                idleTimer = RandomIdleSoundInterval();
            }
            walkTimer -= Time.deltaTime;
            if (walkTimer <= 0f)
            {
                _executor.SoundManager.PlaySound("Walk");
                walkTimer = walkTimeInterval;
            }
            // check if arrive next patrol point
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
                    _executor.SoundManager.StopSound();
                    return Node.Status.SUCCESS;
                }
            }
            return Node.Status.RUNNING;
        }

        public void OnStatusRunning()
        {
            //Debug.Log("Strategy Patrol running");
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
            idleTimer = idleSoundInterval;
            walkTimer = walkTimeInterval;
        }

        public void OnStatusFailure()
        {
            _executor.SoundManager.StopSound();
            _executor.Agent.ResetPath();
            _executor.Animator.SetBool("IsIdle", false);
            _executor.Animator.SetBool("IsPatrolling", false);
            idleTimer = idleSoundInterval;
            walkTimer = walkTimeInterval;
        }

        Vector3 GetNextPatrolPoint()
        {
            // return 0 point if only exist one patrol point
            // (usually happens when no patrol point set)
            if (_patrolPoints.Count == 1)
            {
                return _patrolPoints[0].position;
            }
            int nextPatrolIndex = (_curPatrolIndex + 1) % _patrolPoints.Count;
            Vector3 nextpoint = _patrolPoints[nextPatrolIndex].position;
            //Debug.Log($"next patrol point index: {_curPatrolIndex}");
            return nextpoint;
        }
        void MoveToNextPoint()
        {
            MoveToDestination(GetNextPatrolPoint());
            _curPatrolIndex = (_curPatrolIndex + 1) % _patrolPoints.Count;
            //_executor.Animator.SetBool("IsMoving", true);

        }
        public void HandleRestartRequest()
        {
            OnStatusRunning(); 
            //_curPatrolIndex = -1;
            //MoveToNextPoint();
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