using Assets.AI.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AI.BehaviourTree
{

    public class AI_DetonateAttack : MonoBehaviour, IBehaviourSubTree
    {
        Node _behaviorTree;

        EnemyStateExecutor _executor;
        AIMethods _methods;

        public float RecoveryTime = 1f;
        public float DetonateRange = 5f;
        public float scaleMult = 1.85f;

        Vector3 _originScale;
        bool _detonateFailed = false;
        float _timer;

        private void Start()
        {
            _originScale = gameObject.transform.localScale;
        }
        public Node BuildBehaviorTree(EnemyStateExecutor executor, AIMethods methods)
        {
            _executor = executor;
            _methods = methods;
            _behaviorTree = new Sequence("ST_Attack");
            _behaviorTree.AddChild(new Leaf("CanAttackPlayer",
                new Condition(() => { return _methods.CanAttackPlayer(); })));
            _behaviorTree.AddChild(new Leaf("Detonate Countdown",
                new Strategy_AI_DetonateAttack(
                    methods, executor, DetonateRange, scaleMult, _originScale,
                    () => { 
                        _detonateFailed = true;
                        _timer = 0;
                    },
                    () => { AfterExplode(); }
            )));
            return _behaviorTree;
        }

        private void Update()
        {
            if (_detonateFailed)
            {
                _timer += Time.deltaTime;
                gameObject.transform.localScale = Vector3.Lerp(
                    Vector3.one * scaleMult, _originScale, _timer / RecoveryTime);
                if (_timer >= RecoveryTime)
                {
                    _detonateFailed = false;
                    _timer = 0;
                    gameObject.transform.localScale = _originScale;
                }
            }
        }

        void AfterExplode()
        {
            _isExploding = true;
            _executor.Animator.gameObject.SetActive(false);
            if (_methods.IsPlayerInRange(DetonateRange))
            {
                _executor.DamagePlayer();
            }
            _executor.OnDying();
            StartCoroutine(ExtendMethods.DelayAction(1.9f, () =>
            {
                _executor.Animator.gameObject.SetActive(true);
                _behaviorTree.Reset();
                _isExploding = false;
                gameObject.transform.localScale = Vector3.one * 1.5f;
            }));
        }
        bool _isExploding = false;
        void OnDrawGizmos()
        {
            if (_isExploding)
            {
                Gizmos.color = Color.red; // 设置绘制颜色
                Gizmos.DrawWireSphere(transform.position, DetonateRange); // 绘制一个球形范围
            }
        }
    }

    public class Strategy_AI_DetonateAttack : IStrategy
    {
        readonly EnemyStateExecutor _executor;
        readonly AIMethods _methods;
        float _detonateTime;
        float _timer = 0;
        float _detonateRange;
        float _scaleMult;
        Vector3 _originScale;
        Action _failedCallback;
        Action _successCallback;

        bool detonated = false;

        public Strategy_AI_DetonateAttack(
            AIMethods methods,
            EnemyStateExecutor executor, 
            float detonateRange, float sizeMult, Vector3 originScale,
            Action failedCallback, Action successCallback)
        {
            _methods = methods;
            _executor = executor;
            _detonateTime = executor.AttackTime;
            _detonateRange = detonateRange;
            _scaleMult = sizeMult;
            _originScale = originScale;
            _failedCallback = failedCallback;
            _successCallback = successCallback;
        }

        public Node.Status Evaluate()
        {
            if (detonated) return Node.Status.RUNNING;
            _timer += Time.deltaTime;
            _executor.gameObject.transform.localScale = Vector3.Lerp(
                _originScale, Vector3.one * _scaleMult, _timer / _detonateTime);

            if (_timer >= _detonateTime)
            {
                detonated = true;
                _successCallback();
                return Node.Status.SUCCESS;
            }
            if (!_methods.IsPlayerInRange(_detonateRange * 1.5f))
            {
                _failedCallback();
                return Node.Status.FAILURE;
            }
            return Node.Status.RUNNING;
        }

        public void OnStatusSuccess()
        {
        }
        public void OnStatusRunning()
        {
            if (detonated) return;
            Initialize();
            _executor.Animator.SetBool("IsIdle", true);
            _executor.Agent.isStopped = true;
        }

        public void OnStatusFailure()
        {
            if (detonated) return;
            Initialize();
            _executor.Agent.isStopped = false;
        }

        void Initialize()
        {
            _methods.ResetAllAnimationTriggers();
            _executor.Agent.ResetPath();
            _timer = 0;
        }

        public void Reset()
        {
            //Debug.Log("Reset Strategy_AI_DetonateAttack");
            detonated = false;
            _timer = 0;
        }
    }
}
